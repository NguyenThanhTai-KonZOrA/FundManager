import React, { useState, useEffect, useCallback } from 'react';
import {
    Box, Card, CardContent, Typography, Button, Table, TableBody, TableCell,
    TableContainer, TableHead, TableRow, Paper, Chip, IconButton, Dialog,
    DialogTitle, DialogContent, DialogActions, TextField, Switch, FormControlLabel,
    CircularProgress, Alert, Tooltip, Stack, Select, MenuItem, FormControl, InputLabel,
    Collapse, Tabs, Tab, TablePagination, InputAdornment,
} from '@mui/material';
import {
    Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, Refresh as RefreshIcon,
    ExpandMore as ExpandMoreIcon, ExpandLess as ExpandLessIcon,
    Translate as TranslateIcon, History as HistoryIcon, Visibility as VisibilityIcon,
    Search as SearchIcon,
} from '@mui/icons-material';
import AdminLayout from '../../components/layout/AdminLayout';
import { useSetPageTitle } from '../../hooks/useSetPageTitle';
import { PAGE_TITLES } from '../../constants/pageTitles';
import { documentTemplateService } from '../../services/documentTemplateService';
import { languageService } from '../../services/masterDataService';
import type { LanguageResponse } from '../../types/masterDataType';
import TranslationViewer from '../../components/TranslationViewer';
import { FormatUtcTime } from '../../utils/formatUtcTime';
import type {
    DocumentTemplateBriefResponse,
    CreateDocumentTemplateRequest,
    UpdateDocumentTemplateRequest,
} from '../../types/documentTemplateType';
import { DocumentType, DOCUMENT_TYPE_LABELS } from '../../types/documentTemplateType';
import type {
    DocumentTemplateTranslationResponse,
    DocumentTemplateVersionHistoryResponse,
    UpsertDocumentTemplateTranslationRequest,
} from '../../types/documentTemplateType';
import { HtmlContentEditor } from '../../components/HtmlContentEditor';
import { useTheme } from '@mui/material/styles';
import { normalizeAttachmentUrl } from '../../utils/attachmentUtils';
import { getApiBase } from '../../utils/envConfig';

const DOCUMENT_TYPES = Object.entries(DOCUMENT_TYPE_LABELS).map(([k, v]) => ({
    value: Number(k) as DocumentType,
    label: v,
}));

// Language options are loaded from the API at runtime

const EMPTY_CREATE: CreateDocumentTemplateRequest = {
    title: '', documentType: DocumentType.Other, description: '', content: '', outletId: null,
};

interface ExpandedRowState {
    open: boolean;
    tab: number; // 0 = translations, 1 = history
    translations: DocumentTemplateTranslationResponse[];
    versionHistories: DocumentTemplateVersionHistoryResponse[];
    loading: boolean;
}

export default function AdminDocumentTemplatePage() {
    useSetPageTitle(PAGE_TITLES.DOCUMENT_TEMPLATES);
    const muiTheme = useTheme();
    const [templates, setTemplates] = useState<DocumentTemplateBriefResponse[]>([]);
    const [filteredTemplates, setFilteredTemplates] = useState<DocumentTemplateBriefResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Search & Pagination states
    const [searchQuery, setSearchQuery] = useState('');
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [saving, setSaving] = useState(false);
    const [languages, setLanguages] = useState<LanguageResponse[]>([]);
    const [langLoading, setLangLoading] = useState(false);
    const [viewerOpen, setViewerOpen] = useState(false);
    const [viewerPayload, setViewerPayload] = useState<{ title?: string; languageCode?: string; content?: string; description?: string | null } | null>(null);
    const langMap = React.useMemo(() => Object.fromEntries(languages.map(l => [l.code, l])), [languages]);

    // ─── CRUD dialog state ──────────────────────────────────────────────────
    const [createOpen, setCreateOpen] = useState(false);
    const [editOpen, setEditOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);

    const [createForm, setCreateForm] = useState<CreateDocumentTemplateRequest>(EMPTY_CREATE);
    const [editForm, setEditForm] = useState<UpdateDocumentTemplateRequest>({
        id: 0, title: '', documentType: DocumentType.Other,
        description: '', content: '', outletId: null, isActive: true,
    });
    const [deleteId, setDeleteId] = useState<number | null>(null);

    // ─── Expandable rows ────────────────────────────────────────────────────
    const [expandedRows, setExpandedRows] = useState<Record<number, ExpandedRowState>>({});

    const toggleExpand = async (t: DocumentTemplateBriefResponse) => {
        const prev = expandedRows[t.id];
        if (prev?.open) {
            setExpandedRows(r => ({ ...r, [t.id]: { ...prev, open: false } }));
            return;
        }
        // Open and lazy-load if not yet loaded
        if (prev && prev.translations.length === 0 && prev.versionHistories.length === 0 && !prev.loading) {
            setExpandedRows(r => ({ ...r, [t.id]: { ...prev, open: true, loading: true } }));
            try {
                const [trans, hist] = await Promise.all([
                    documentTemplateService.getTranslationsAsync(t.id),
                    documentTemplateService.getHistoryAsync(t.id),
                ]);
                setExpandedRows(r => ({ ...r, [t.id]: { ...r[t.id], translations: trans, versionHistories: hist, loading: false } }));
            } catch {
                setExpandedRows(r => ({ ...r, [t.id]: { ...r[t.id], loading: false } }));
            }
        } else if (!prev) {
            const init: ExpandedRowState = { open: true, tab: 0, translations: t.translations ?? [], versionHistories: t.versionHistories ?? [], loading: false };
            setExpandedRows(r => ({ ...r, [t.id]: init }));
        } else {
            setExpandedRows(r => ({ ...r, [t.id]: { ...prev, open: true } }));
        }
    };

    const setRowTab = (id: number, tab: number) =>
        setExpandedRows(r => ({ ...r, [id]: { ...r[id], tab } }));

    // ─── Translation dialog ─────────────────────────────────────────────────
    const [transOpen, setTransOpen] = useState(false);
    const [transTarget, setTransTarget] = useState<DocumentTemplateBriefResponse | null>(null);
    const [editingTrans, setEditingTrans] = useState<DocumentTemplateTranslationResponse | null>(null);
    const [transForm, setTransForm] = useState<UpsertDocumentTemplateTranslationRequest>({
        documentTemplateId: 0, languageCode: 'en', title: '', description: '', content: '',
    });

    // ─── Load ───────────────────────────────────────────────────────────────
    const load = useCallback(async () => {
        setLoading(true); setError(null);
        try {
            const data = await documentTemplateService.getListAsync();
            setTemplates(data);
            setFilteredTemplates(data);
        } catch {
            setError('Failed to load document templates.');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => { load(); }, [load]);

    // Filter templates based on search query
    useEffect(() => {
        if (!searchQuery.trim()) {
            setFilteredTemplates(templates);
            setPage(0);
            return;
        }

        const query = searchQuery.toLowerCase();
        const filtered = templates.filter(t =>
            t.title.toLowerCase().includes(query) ||
            t.description?.toLowerCase().includes(query) ||
            DOCUMENT_TYPE_LABELS[t.documentType]?.toLowerCase().includes(query) ||
            t.id.toString().includes(query) ||
            t.version.toString().includes(query)
        );
        setFilteredTemplates(filtered);
        setPage(0);
    }, [searchQuery, templates]);

    // Get paginated data
    const paginatedTemplates = filteredTemplates.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

    const handleChangePage = (_: unknown, newPage: number) => setPage(newPage);
    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const loadLanguages = useCallback(async () => {
        setLangLoading(true);
        try {
            setLanguages(await languageService.getAllAsync());
        } catch {
            // ignore
        } finally {
            setLangLoading(false);
        }
    }, []);

    useEffect(() => { loadLanguages(); }, [loadLanguages]);

    // ─── CRUD handlers ──────────────────────────────────────────────────────
    const openEdit = async (id: number) => {
        try {
            const t = await documentTemplateService.getByIdAsync(id);
            setEditForm({
                id: t.id, title: t.title, documentType: t.documentType,
                description: t.description, content: t.content,
                outletId: t.outletId, isActive: t.isActive,
            });
            setEditOpen(true);
        } catch { setError('Failed to load template.'); }
    };

    const handleCreate = async () => {
        setSaving(true);
        try {
            await documentTemplateService.createAsync(createForm);
            setCreateOpen(false);
            setCreateForm(EMPTY_CREATE);
            await load();
        } catch { setError('Failed to create document template.'); }
        finally { setSaving(false); }
    };

    const handleUpdate = async () => {
        setSaving(true);
        try {
            await documentTemplateService.updateAsync(editForm);
            setEditOpen(false);
            await load();
        } catch { setError('Failed to update document template.'); }
        finally { setSaving(false); }
    };

    const handleDelete = async () => {
        if (deleteId == null) return;
        setSaving(true);
        try {
            await documentTemplateService.deleteAsync(deleteId);
            setDeleteOpen(false); setDeleteId(null);
            await load();
        } catch { setError('Failed to delete document template.'); }
        finally { setSaving(false); }
    };

    // ─── Translation handlers ───────────────────────────────────────────────
    const openAddTrans = (t: DocumentTemplateBriefResponse) => {
        setTransTarget(t);
        setEditingTrans(null);
        setTransForm({ documentTemplateId: t.id, languageCode: 'en', title: '', description: '', content: '' });
        setTransOpen(true);
    };

    const openEditTrans = (t: DocumentTemplateBriefResponse, tr: DocumentTemplateTranslationResponse) => {
        setTransTarget(t);
        setEditingTrans(tr);
        setTransForm({
            documentTemplateId: t.id, languageCode: tr.languageCode,
            title: tr.title, description: tr.description ?? '',
            content: tr.content,
        });
        setTransOpen(true);
    };

    const handleSaveTrans = async () => {
        setSaving(true);
        try {
            await documentTemplateService.upsertTranslationAsync(transForm);
            setTransOpen(false);
            // Refresh translations in expanded row
            if (transTarget) {
                const trans = await documentTemplateService.getTranslationsAsync(transTarget.id);
                setExpandedRows(r => ({
                    ...r,
                    [transTarget.id]: { ...r[transTarget.id], translations: trans },
                }));
            }
            await load();
        } catch { setError('Failed to save translation.'); }
        finally { setSaving(false); }
    };

    // ─── Render ─────────────────────────────────────────────────────────────
    return (
        <AdminLayout>
            <Box sx={{ p: 3 }}>
                <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
                    <TextField
                        size="small"
                        placeholder="Search templates..."
                        value={searchQuery}
                        onChange={e => setSearchQuery(e.target.value)}
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <SearchIcon fontSize="small" />
                                </InputAdornment>
                            ),
                        }}
                        sx={{ width: 300 }}
                    />
                    <Stack direction="row" spacing={1}>
                        <Button startIcon={<RefreshIcon />} variant="outlined" onClick={load}>Refresh</Button>
                        <Button startIcon={<AddIcon />} variant="contained"
                            onClick={() => { setCreateForm(EMPTY_CREATE); setCreateOpen(true); }}>
                            New Template
                        </Button>
                    </Stack>
                </Stack>

                {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}

                <Card>
                    <CardContent sx={{ p: 0 }}>
                        {loading ? (
                            <Box display="flex" justifyContent="center" p={4}><CircularProgress /></Box>
                        ) : (
                            <TableContainer component={Paper} elevation={0}>
                                <Table>
                                    <TableHead>
                                        <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                                            <TableCell width={50}></TableCell>
                                            <TableCell width={50}>#</TableCell>
                                            <TableCell>Title</TableCell>
                                            <TableCell>Type</TableCell>
                                            {/* <TableCell>Version</TableCell> */}
                                            <TableCell>Outlet</TableCell>
                                            <TableCell>Status</TableCell>
                                            <TableCell>Last Updated</TableCell>
                                            <TableCell align="right">Actions</TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {filteredTemplates.length === 0 ? (
                                            <TableRow>
                                                <TableCell colSpan={8} align="center">
                                                    {searchQuery ? 'No templates match your search.' : 'No document templates found.'}
                                                </TableCell>
                                            </TableRow>
                                        ) : paginatedTemplates.map((t, idx) => {
                                            const row = expandedRows[t.id];
                                            const isOpen = row?.open ?? false;
                                            return (
                                                <React.Fragment key={t.id}>
                                                    {/* ─── Main row ─── */}
                                                    <TableRow hover>
                                                        <TableCell>
                                                            <IconButton size="small" onClick={() => toggleExpand(t)}>
                                                                {isOpen ? <ExpandLessIcon fontSize="small" /> : <ExpandMoreIcon fontSize="small" />}
                                                            </IconButton>
                                                        </TableCell>
                                                        <TableCell>{idx + 1}</TableCell>
                                                        <TableCell>
                                                            <Typography fontWeight={500}>
                                                                {t.title}
                                                                <Chip label={`Version ${t.version}`} size="small" variant="outlined" color="primary" sx={{ m: 0.75 }} />
                                                            </Typography>
                                                            {t.description && (
                                                                <Typography variant="caption" color="text.secondary">{t.description}</Typography>
                                                            )}
                                                        </TableCell>
                                                        <TableCell>
                                                            <Chip label={t.documentTypeName} size="small" variant="outlined" color="primary" />
                                                        </TableCell>
                                                        {/* <TableCell>v{t.version}</TableCell> */}
                                                        <TableCell>{t.outletName ?? <Typography variant="caption" color="text.secondary">Global</Typography>}</TableCell>
                                                        <TableCell>
                                                            <Chip label={t.isActive ? 'Active' : 'Inactive'} size="small"
                                                                color={t.isActive ? 'success' : 'default'} />
                                                        </TableCell>
                                                        <TableCell>{FormatUtcTime.formatDateDDMMMYYYYWithoutTime(t.updatedAt)} <br />by <strong>{t.updatedBy}</strong></TableCell>
                                                        <TableCell align="right">
                                                            <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                                                                <Tooltip title="Translations & History">
                                                                    <IconButton size="small" color="secondary" onClick={() => toggleExpand(t)}>
                                                                        <TranslateIcon fontSize="small" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                                <Tooltip title="Edit">
                                                                    <IconButton size="small" onClick={() => openEdit(t.id)}>
                                                                        <EditIcon fontSize="small" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                                <Tooltip title="Delete">
                                                                    <IconButton size="small" color="error"
                                                                        onClick={() => { setDeleteId(t.id); setDeleteOpen(true); }}>
                                                                        <DeleteIcon fontSize="small" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                            </Stack>
                                                        </TableCell>
                                                    </TableRow>

                                                    {/* ─── Expanded row ─── */}
                                                    <TableRow>
                                                        <TableCell colSpan={8} sx={{ py: 0, borderBottom: isOpen ? undefined : 'none' }}>
                                                            <Collapse in={isOpen} timeout="auto" unmountOnExit>
                                                                <Box sx={{ py: 2, px: 4, bgcolor: 'grey.50' }}>
                                                                    {row?.loading ? (
                                                                        <CircularProgress size={20} />
                                                                    ) : (
                                                                        <>
                                                                            <Tabs value={row?.tab ?? 0}
                                                                                onChange={(_, v) => setRowTab(t.id, v)}
                                                                                sx={{ mb: 1.5 }}>
                                                                                <Tab icon={<TranslateIcon fontSize="small" />}
                                                                                    iconPosition="start"
                                                                                    label={`Translations (${row?.translations.length ?? 0})`} />
                                                                                <Tab icon={<HistoryIcon fontSize="small" />}
                                                                                    iconPosition="start"
                                                                                    label={`Version History (${row?.versionHistories.length ?? 0})`} />
                                                                            </Tabs>

                                                                            {/* Translations tab */}
                                                                            {(row?.tab ?? 0) === 0 && (
                                                                                <>
                                                                                    <Stack direction="row" justifyContent="flex-end" mb={1}>
                                                                                        <Button size="small" startIcon={<AddIcon />}
                                                                                            onClick={() => openAddTrans(t)}>
                                                                                            Add Language
                                                                                        </Button>
                                                                                    </Stack>
                                                                                    {(row?.translations ?? []).length === 0 ? (
                                                                                        <Typography variant="caption" color="text.disabled">
                                                                                            No translations yet.
                                                                                        </Typography>
                                                                                    ) : (
                                                                                        <Table size="small">
                                                                                            <TableHead>
                                                                                                <TableRow sx={{
                                                                                                    bgcolor: `${muiTheme.palette.primary.main}`,
                                                                                                    '& .MuiTableCell-root': { color: '#fff', fontWeight: 600, borderBottom: 'none' },
                                                                                                }}>
                                                                                                    <TableCell>Language</TableCell>
                                                                                                    <TableCell>Title</TableCell>
                                                                                                    <TableCell>Description</TableCell>
                                                                                                    <TableCell>Content</TableCell>
                                                                                                    <TableCell>Last Updated</TableCell>
                                                                                                    <TableCell>Updated By</TableCell>
                                                                                                    <TableCell align="right">Action</TableCell>
                                                                                                </TableRow>
                                                                                            </TableHead>
                                                                                            <TableBody>
                                                                                                {row!.translations.map(tr => (
                                                                                                    <TableRow key={tr.id} hover>
                                                                                                        <TableCell>
                                                                                                            <Stack direction="row" alignItems="center" spacing={1}>
                                                                                                                <span>
                                                                                                                    <img src={langMap[tr.languageCode]?.flagEmoji ? getApiBase() + langMap[tr.languageCode]?.flagEmoji : ''} alt="" style={{ width: 25, height: 16, objectFit: 'cover', marginTop: 4 }} />
                                                                                                                </span>
                                                                                                                <Typography variant="body2">{langMap[tr.languageCode]?.name ?? tr.languageCode.toUpperCase()}</Typography>
                                                                                                            </Stack>
                                                                                                        </TableCell>
                                                                                                        <TableCell>{tr.title}</TableCell>
                                                                                                        <TableCell>
                                                                                                            <Typography variant="caption" color="text.secondary">
                                                                                                                {tr.description
                                                                                                                    ? tr.description.substring(0, 40) + (tr.description.length > 40 ? '…' : '')
                                                                                                                    : '—'}
                                                                                                            </Typography>
                                                                                                        </TableCell>
                                                                                                        <TableCell>
                                                                                                            <Chip label={tr.content ? '✓ Has content' : '—'} size="small"
                                                                                                                color={tr.content ? 'success' : 'default'} variant="outlined" />
                                                                                                        </TableCell>
                                                                                                        <TableCell>{FormatUtcTime.formatDateTime(tr.updatedAt)}</TableCell>
                                                                                                        <TableCell>
                                                                                                            <Typography variant="caption">{tr.updatedBy}</Typography>
                                                                                                        </TableCell>
                                                                                                        <TableCell align="right">
                                                                                                            <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                                                                                                                <Tooltip title="View Translation">
                                                                                                                    <IconButton size="small" color="info" onClick={() => { setViewerPayload({ title: tr.title, languageCode: tr.languageCode, content: tr.content, description: tr.description }); setViewerOpen(true); }}>
                                                                                                                        <VisibilityIcon fontSize="small" />
                                                                                                                    </IconButton>
                                                                                                                </Tooltip>
                                                                                                                <Tooltip title="Edit Translation">
                                                                                                                    <IconButton size="small" color="primary" onClick={() => openEditTrans(t, tr)}>
                                                                                                                        <EditIcon fontSize="small" />
                                                                                                                    </IconButton>
                                                                                                                </Tooltip>
                                                                                                            </Stack>
                                                                                                        </TableCell>
                                                                                                    </TableRow>
                                                                                                ))}
                                                                                            </TableBody>
                                                                                        </Table>
                                                                                    )}
                                                                                </>
                                                                            )}

                                                                            {/* Version History tab */}
                                                                            {row?.tab === 1 && (
                                                                                (row?.versionHistories ?? []).length === 0 ? (
                                                                                    <Typography variant="caption" color="text.disabled">
                                                                                        No version history yet.
                                                                                    </Typography>
                                                                                ) : (
                                                                                    <Table size="small">
                                                                                        <TableHead>
                                                                                            <TableRow sx={{
                                                                                                bgcolor: `${muiTheme.palette.primary.main}`,
                                                                                                '& .MuiTableCell-root': { color: '#fff', fontWeight: 600, borderBottom: 'none' },
                                                                                            }}>
                                                                                                <TableCell>Version</TableCell>
                                                                                                <TableCell>Title</TableCell>
                                                                                                <TableCell>Updated At</TableCell>
                                                                                                <TableCell>Updated By</TableCell>
                                                                                                <TableCell>Note</TableCell>
                                                                                            </TableRow>
                                                                                        </TableHead>
                                                                                        <TableBody>
                                                                                            {row!.versionHistories.map(h => (
                                                                                                <TableRow key={h.id} hover>
                                                                                                    <TableCell>
                                                                                                        <Chip label={`v${h.version}`} size="small" color="primary" variant="outlined" />
                                                                                                    </TableCell>
                                                                                                    <TableCell>{h.title}</TableCell>
                                                                                                    <TableCell>{FormatUtcTime.formatDateTime(h.updatedAt)}</TableCell>
                                                                                                    <TableCell>
                                                                                                        <Typography variant="caption">{h.updatedBy}</Typography>
                                                                                                    </TableCell>
                                                                                                    <TableCell>
                                                                                                        <Typography variant="caption" color="text.secondary">
                                                                                                            {h.changeNote ?? '—'}
                                                                                                        </Typography>
                                                                                                    </TableCell>
                                                                                                </TableRow>
                                                                                            ))}
                                                                                        </TableBody>
                                                                                    </Table>
                                                                                )
                                                                            )}
                                                                        </>
                                                                    )}
                                                                </Box>
                                                            </Collapse>
                                                        </TableCell>
                                                    </TableRow>
                                                </React.Fragment>
                                            );
                                        })}
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        )}
                        {!loading && filteredTemplates.length > 0 && (
                            <TablePagination
                                component="div"
                                count={filteredTemplates.length}
                                page={page}
                                onPageChange={handleChangePage}
                                rowsPerPage={rowsPerPage}
                                onRowsPerPageChange={handleChangeRowsPerPage}
                                rowsPerPageOptions={[5, 10, 25, 50]}
                            />
                        )}
                    </CardContent>
                </Card>

                {/* ─── Create Dialog ──────────────────────────────────────────────────── */}
                <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="lg" fullWidth>
                    <DialogTitle>Create Document Template</DialogTitle>
                    <DialogContent dividers>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Title" fullWidth required
                                value={createForm.title}
                                onChange={e => setCreateForm(f => ({ ...f, title: e.target.value }))} />
                            <FormControl fullWidth required>
                                <InputLabel>Document Type</InputLabel>
                                <Select label="Document Type" value={createForm.documentType}
                                    onChange={e => setCreateForm(f => ({ ...f, documentType: e.target.value as DocumentType }))}>
                                    {DOCUMENT_TYPES.map(dt => (
                                        <MenuItem key={dt.value} value={dt.value}>{dt.label}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <TextField label="Description" fullWidth multiline rows={2}
                                value={createForm.description}
                                onChange={e => setCreateForm(f => ({ ...f, description: e.target.value }))} />
                            <Box>
                                <Typography variant="subtitle2" mb={1} color="text.secondary">Content (HTML)</Typography>
                                <HtmlContentEditor
                                    initialContent={createForm.content}
                                    title={createForm.title}
                                    onSave={async (html) => setCreateForm(f => ({ ...f, content: html }))}
                                />
                            </Box>
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setCreateOpen(false)}>Cancel</Button>
                        <Button variant="contained" onClick={handleCreate} disabled={saving || !createForm.title}>
                            {saving ? <CircularProgress size={20} /> : 'Create'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Edit Dialog ─────────────────────────────────────────────────────── */}
                <Dialog open={editOpen} onClose={() => setEditOpen(false)} maxWidth="lg" fullWidth>
                    <DialogTitle>Edit Document Template</DialogTitle>
                    <DialogContent dividers>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Title" fullWidth required
                                value={editForm.title}
                                onChange={e => setEditForm(f => ({ ...f, title: e.target.value }))} />
                            <FormControl fullWidth required>
                                <InputLabel>Document Type</InputLabel>
                                <Select label="Document Type" value={editForm.documentType}
                                    onChange={e => setEditForm(f => ({ ...f, documentType: e.target.value as DocumentType }))}>
                                    {DOCUMENT_TYPES.map(dt => (
                                        <MenuItem key={dt.value} value={dt.value}>{dt.label}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <TextField label="Description" fullWidth multiline rows={2}
                                value={editForm.description}
                                onChange={e => setEditForm(f => ({ ...f, description: e.target.value }))} />
                            <Box>
                                <Typography variant="subtitle2" mb={1} color="text.secondary">Content (HTML)</Typography>
                                <HtmlContentEditor
                                    key={editForm.id}
                                    initialContent={editForm.content}
                                    title={editForm.title}
                                    onSave={async (html) => setEditForm(f => ({ ...f, content: html }))}
                                />
                            </Box>
                            <FormControlLabel
                                control={
                                    <Switch checked={editForm.isActive}
                                        onChange={e => setEditForm(f => ({ ...f, isActive: e.target.checked }))} />
                                }
                                label="Active"
                            />
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setEditOpen(false)}>Cancel</Button>
                        <Button variant="contained" onClick={handleUpdate} disabled={saving || !editForm.title}>
                            {saving ? <CircularProgress size={20} /> : 'Save Changes'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Delete Confirm ──────────────────────────────────────────────────── */}
                <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
                    <DialogTitle>Delete Document Template</DialogTitle>
                    <DialogContent>
                        <Typography>Are you sure you want to delete this document template? This action cannot be undone.</Typography>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
                        <Button variant="contained" color="error" onClick={handleDelete} disabled={saving}>
                            {saving ? <CircularProgress size={20} /> : 'Delete'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Translation Upsert Dialog ──────────────────────────────────────── */}
                <Dialog open={transOpen} onClose={() => setTransOpen(false)} maxWidth="lg" fullWidth>
                    <DialogTitle>
                        {editingTrans
                            ? `Edit Translation — ${editingTrans.languageCode.toUpperCase()} · ${transTarget?.title}`
                            : `Add Language Translation · ${transTarget?.title}`}
                    </DialogTitle>
                    <DialogContent dividers>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <FormControl sx={{ width: 240 }}>
                                <InputLabel>Language</InputLabel>
                                <Select label="Language" value={transForm.languageCode}
                                    disabled={!!editingTrans || langLoading}
                                    onChange={e => setTransForm(f => ({ ...f, languageCode: e.target.value }))}>
                                    {(languages.length ? languages : [{ code: transForm.languageCode, name: transForm.languageCode } as any]).map((l: any) => (
                                        <MenuItem key={l.code} value={l.code}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                                <span>
                                                    <img src={l.flagEmoji ? getApiBase() + l.flagEmoji : ''} alt="" style={{ width: 25, height: 16, objectFit: 'cover', marginTop: 4 }} />
                                                </span>
                                                <span>{l.name ?? l.label ?? l.code}  ({l.code})</span>
                                            </Box>
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <TextField label="Title" fullWidth required
                                value={transForm.title}
                                onChange={e => setTransForm(f => ({ ...f, title: e.target.value }))} />
                            <TextField label="Description (optional)" fullWidth multiline rows={2}
                                value={transForm.description ?? ''}
                                onChange={e => setTransForm(f => ({ ...f, description: e.target.value }))} />
                            <Box>
                                <Typography variant="subtitle2" mb={1} color="text.secondary">
                                    Content HTML ({transForm.languageCode.toUpperCase()})
                                </Typography>
                                <HtmlContentEditor
                                    key={`trans-${transForm.languageCode}-${transTarget?.id}`}
                                    initialContent={transForm.content}
                                    title={transForm.title}
                                    onSave={async (html) => setTransForm(f => ({ ...f, content: html }))}
                                />
                            </Box>
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setTransOpen(false)} disabled={saving}>Cancel</Button>
                        <Button variant="contained" onClick={handleSaveTrans}
                            disabled={saving || !transForm.title}>
                            {saving ? <CircularProgress size={18} /> : 'Save Translation'}
                        </Button>
                    </DialogActions>
                </Dialog>
                <TranslationViewer
                    open={viewerOpen}
                    onClose={() => setViewerOpen(false)}
                    title={viewerPayload?.title}
                    languageCode={viewerPayload?.languageCode}
                    contentHtml={viewerPayload?.content}
                    description={viewerPayload?.description}
                />
            </Box>
        </AdminLayout >
    );
}

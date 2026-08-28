import React, { useState, useEffect, useCallback } from 'react';
import {
    Box, Card, CardContent, Typography, Button, Table, TableBody, TableCell,
    TableContainer, TableHead, TableRow, Chip, IconButton, Dialog,
    DialogTitle, DialogContent, DialogActions, TextField, Switch, FormControlLabel,
    CircularProgress, Alert, Tooltip, Stack, Divider, Select, MenuItem,
    FormControl, InputLabel, Checkbox, Accordion, AccordionSummary, AccordionDetails,
    Collapse, Tabs, Tab, TablePagination, InputAdornment,
} from '@mui/material';
import {
    Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, Refresh as RefreshIcon,
    Visibility as VisibilityIcon, ExpandMore as ExpandMoreIcon, ExpandLess as ExpandLessIcon,
    DragIndicator as DragIcon, Translate as TranslateIcon, Preview as PreviewIcon,
    History as HistoryIcon, Search as SearchIcon,
} from '@mui/icons-material';
import AdminLayout from '../../components/layout/AdminLayout';
import { useSetPageTitle } from '../../hooks/useSetPageTitle';
import { PAGE_TITLES } from '../../constants/pageTitles';
import { formTemplateService } from '../../services/formWorkflowService';
import { languageService } from '../../services/masterDataService';
import type { LanguageResponse } from '../../types/masterDataType';
import TranslationViewer from '../../components/TranslationViewer';
import { FormatUtcTime } from '../../utils/formatUtcTime';
import {
    type FormTemplateBriefResponse, type FormTemplateResponse, type FormQuestionResponse,
    type FormTemplateTranslationResponse, type FormTemplateVersionHistoryResponse,
    type CreateFormTemplateRequest, type UpdateFormTemplateRequest,
    type CreateFormQuestionRequest, type UpdateFormQuestionRequest,
    type UpsertFormTemplateTranslationRequest,
    type TransQuestionsDraftData,
    QuestionType
} from '../../types/formTemplateType';
import { useTheme } from '@mui/material/styles';
import { normalizeAttachmentUrl } from '../../utils/attachmentUtils';
import { getApiBase } from '../../utils/envConfig';
import { ImageTypeEnum, type ApplicationImageResponse } from '../../types/applicationImageType';
import { applicationImageService } from '../../services/applicationImageService';
const QUESTION_TYPE_LABELS: Record<number, string> = {
    1: 'Text Input',
    2: 'Single Choice',
    3: 'Multiple Choice',
    4: 'Yes / No',
    5: 'Rating',
    6: 'Date',
};

const EMPTY_TEMPLATE: CreateFormTemplateRequest = {
    title: '', description: '', logoUrl: '', footerText: '', agreementText: '',
};

const EMPTY_QUESTION: CreateFormQuestionRequest = {
    formTemplateId: 0, questionText: '', questionType: QuestionType.TextInput,
    isRequired: false, hasFollowUpText: false, followUpLabel: null, followUpTriggerOption: null,
    options: [],
};

export default function AdminFormTemplatePage() {
    useSetPageTitle(PAGE_TITLES.FORM_TEMPLATES);
    const muiTheme = useTheme();
    const [templates, setTemplates] = useState<FormTemplateBriefResponse[]>([]);
    const [filteredTemplates, setFilteredTemplates] = useState<FormTemplateBriefResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Search & Pagination states
    const [searchQuery, setSearchQuery] = useState('');
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);

    // Template CRUD dialogs
    const [createOpen, setCreateOpen] = useState(false);
    const [editOpen, setEditOpen] = useState(false);
    const [detailOpen, setDetailOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);

    const [createForm, setCreateForm] = useState<CreateFormTemplateRequest>(EMPTY_TEMPLATE);
    const [editForm, setEditForm] = useState<UpdateFormTemplateRequest & { id: number }>({
        id: 0, title: '', description: '', logoUrl: '', footerText: '', agreementText: '', isActive: true,
    });
    const [selectedTemplate, setSelectedTemplate] = useState<FormTemplateResponse | null>(null);
    const [deleteId, setDeleteId] = useState<number | null>(null);
    const [saving, setSaving] = useState(false);

    // Form preview
    const [previewOpen, setPreviewOpen] = useState(false);
    const [previewTemplate, setPreviewTemplate] = useState<FormTemplateResponse | null>(null);

    const [iconsOutlet, setIconsOutlet] = useState<ApplicationImageResponse[]>([]);

    const loadIconOutlet = useCallback(async () => {
        try {
            const icons = await applicationImageService.getByTypeAsync(ImageTypeEnum.Outlet);
            setIconsOutlet(icons);
        } catch { setError('Failed to load icons.'); }
    }, []);

    const openPreview = async (id: number) => {
        try {
            const t = await formTemplateService.getByIdAsync(id);
            setPreviewTemplate(t);
            setPreviewOpen(true);
        } catch { setError('Failed to load preview.'); }
    };

    // ─── Expandable rows (translations + version history) ───────────────────
    interface ExpandedRowState {
        open: boolean;
        tab: number; // 0 = translations, 1 = history
        translations: FormTemplateTranslationResponse[];
        history: FormTemplateVersionHistoryResponse[];
        loading: boolean;
    }
    const [expandedRows, setExpandedRows] = useState<Record<number, ExpandedRowState>>({});

    const toggleExpand = async (t: FormTemplateBriefResponse) => {
        const prev = expandedRows[t.id];
        if (prev?.open) {
            setExpandedRows(r => ({ ...r, [t.id]: { ...prev, open: false } }));
            return;
        }
        if (!prev) {
            // First open: seed from list data, then async-refresh from API
            const init: ExpandedRowState = {
                open: true, tab: 0,
                translations: t.translations ?? [],
                history: t.versionHistories ?? [],
                loading: false,
            };
            setExpandedRows(r => ({ ...r, [t.id]: init }));
            try {
                const [trans, hist] = await Promise.all([
                    formTemplateService.getTranslationsAsync(t.id),
                    formTemplateService.getHistoryAsync(t.id),
                ]);
                setExpandedRows(r => ({ ...r, [t.id]: { ...r[t.id], translations: trans, history: hist } }));
            } catch { /* keep seeded data */ }
        } else {
            setExpandedRows(r => ({ ...r, [t.id]: { ...prev, open: true } }));
        }
    };

    const setRowTab = (id: number, tab: number) =>
        setExpandedRows(r => ({ ...r, [id]: { ...r[id], tab } }));

    // Translation dialog state
    const [transOpen, setTransOpen] = useState(false);
    const [transForm, setTransForm] = useState<UpsertFormTemplateTranslationRequest>({
        formTemplateId: 0, languageCode: 'vi', title: '', description: null, footerText: null, agreementText: '', questionsTranslation: null,
    });
    const [editingTrans, setEditingTrans] = useState<FormTemplateTranslationResponse | null>(null);
    const [transTargetTemplate, setTransTargetTemplate] = useState<FormTemplateBriefResponse | null>(null);
    const [transTemplateQuestions, setTransTemplateQuestions] = useState<FormQuestionResponse[] | null>(null);
    const [transQuestionsDraft, setTransQuestionsDraft] = useState<TransQuestionsDraftData[]>([]);
    const [transEditorMode, setTransEditorMode] = useState<'visual' | 'json'>('visual');
    const [transParseError, setTransParseError] = useState<string | null>(null);
    const [languages, setLanguages] = useState<LanguageResponse[]>([]);
    const [langLoading, setLangLoading] = useState(false);
    const [viewerOpen, setViewerOpen] = useState(false);
    const [viewerPayload, setViewerPayload] = useState<{ title?: string; languageCode?: string; content?: string; description?: string | null; questionsTranslation?: string | null } | null>(null);
    const langMap = React.useMemo(() => Object.fromEntries(languages.map(l => [l.code, l])), [languages]);

    // Question dialogs
    const [addQOpen, setAddQOpen] = useState(false);
    const [editQOpen, setEditQOpen] = useState(false);
    const [deleteQOpen, setDeleteQOpen] = useState(false);
    const [addQForm, setAddQForm] = useState<CreateFormQuestionRequest>(EMPTY_QUESTION);
    const [editQForm, setEditQForm] = useState<UpdateFormQuestionRequest & { id: number }>({
        id: 0, questionText: '', questionType: QuestionType.TextInput,
        isRequired: false, hasFollowUpText: false, followUpLabel: null, followUpTriggerOption: null,
        options: [],
    });
    const [deleteQId, setDeleteQId] = useState<number | null>(null);
    const [optionInput, setOptionInput] = useState('');
    const [editOptionInput, setEditOptionInput] = useState('');

    const load = useCallback(async () => {
        setLoading(true); setError(null);
        try {
            const data = await formTemplateService.getListAsync();
            setTemplates(data);
            setFilteredTemplates(data);
        } catch {
            setError('Failed to load form templates.');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => { load(); loadIconOutlet(); }, [load, loadIconOutlet]);

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

    const openDetail = async (id: number) => {
        try {
            const t = await formTemplateService.getByIdAsync(id);
            setSelectedTemplate(t);
            setDetailOpen(true);
        } catch {
            setError('Failed to load template detail.');
        }
    };

    const openEdit = (t: FormTemplateBriefResponse) => {
        setEditForm({ id: t.id, title: t.title, description: '', logoUrl: '', footerText: '', agreementText: '', isActive: t.isActive });
        // Load full detail for edit
        formTemplateService.getByIdAsync(t.id).then(full => {
            setEditForm({ id: full.id, title: full.title, description: full.description, logoUrl: full.logoUrl, footerText: full.footerText, agreementText: full.agreementText, isActive: full.isActive });
        });
        setEditOpen(true);
    };

    // ─── Template CRUD ─────────────────────────────────────────────────────────

    const handleCreate = async () => {
        setSaving(true);
        try {
            const created = await formTemplateService.createAsync(createForm);
            setCreateOpen(false);
            setCreateForm(EMPTY_TEMPLATE);
            await load();
            // Open preview automatically after creation
            setPreviewTemplate(created as unknown as FormTemplateResponse);
            setPreviewOpen(true);
        } catch {
            setError('Failed to create template.');
        } finally { setSaving(false); }
    };

    const handleUpdate = async () => {
        setSaving(true);
        try {
            await formTemplateService.updateAsync(editForm);
            setEditOpen(false);
            await load();
        } catch {
            setError('Failed to update template.');
        } finally { setSaving(false); }
    };

    const handleDelete = async () => {
        if (!deleteId) return;
        setSaving(true);
        try {
            await formTemplateService.deleteAsync(deleteId);
            setDeleteOpen(false);
            setDeleteId(null);
            await load();
        } catch {
            setError('Failed to delete template.');
        } finally { setSaving(false); }
    };

    // ─── Translation CRUD ─────────────────────────────────────────────────────
    const openAddTrans = async (t: FormTemplateBriefResponse) => {
        setTransTargetTemplate(t);
        setTransForm(
            {
                formTemplateId: t.id,
                languageCode: 'vi',
                title: '',
                description: null,
                footerText: null,
                agreementText: '',
                questionsTranslation: null
            }
        );
        setEditingTrans(null);
        setTransEditorMode('visual');
        setTransParseError(null);
        setTransTemplateQuestions(null);
        setTransQuestionsDraft([]);
        setTransOpen(true);
        try {
            const full = await formTemplateService.getByIdAsync(t.id);
            const qs = full.questions ?? [];
            setTransTemplateQuestions(qs);
            const draft = qs.map(q => (
                {
                    questionId: q.id,
                    questionText: '',
                    hasFollowUpText: q.hasFollowUpText,
                    followUpLabel: '',
                    options: q.options?.map(o => ({
                        optionId: o.id, optionText: o.optionText
                    })) ?? []
                }));
            setTransQuestionsDraft(draft);
        } catch {
            setTransTemplateQuestions([]);
        }
    };

    const openEditTrans = async (t: FormTemplateBriefResponse, tr: FormTemplateTranslationResponse) => {
        setTransTargetTemplate(t);
        setTransForm({
            formTemplateId: t.id, languageCode: tr.languageCode,
            title: tr.title, description: tr.description ?? null,
            footerText: tr.footerText ?? null,
            agreementText: tr.agreementText,
            questionsTranslation: tr.questionsTranslation ?? null,
        });
        setEditingTrans(tr);
        setTransEditorMode('visual');
        setTransParseError(null);
        setTransTemplateQuestions(null);
        setTransQuestionsDraft([]);
        setTransOpen(true);
        try {
            const full = await formTemplateService.getByIdAsync(t.id);
            const qs = full.questions ?? [];
            setTransTemplateQuestions(qs);
            let parsed: any = null;
            if (tr.questionsTranslation) {
                try { parsed = JSON.parse(tr.questionsTranslation); } catch (e) { parsed = null; }
            }
            const draft = qs.map(q => {
                const entry = Array.isArray(parsed) ? parsed.find((p: any) => p.questionId === q.id) : null;
                // Normalize options structure - handle both string[] and object[] formats
                let normalizedOptions: Array<{ optionId: number; optionText: string }> = [];
                if (entry?.options) {
                    if (Array.isArray(entry.options)) {
                        normalizedOptions = entry.options.map((opt: any, idx: number) => {
                            if (typeof opt === 'string') {
                                // If options are strings, map to original question options by index
                                const originalOpt = q.options?.[idx];
                                return { optionId: originalOpt?.id ?? idx, optionText: opt };
                            } else if (opt && typeof opt === 'object') {
                                // If already objects, preserve structure
                                return { optionId: opt.optionId ?? opt.id ?? idx, optionText: opt.optionText ?? opt.text ?? '' };
                            }
                            return { optionId: idx, optionText: String(opt) };
                        });
                    }
                } else {
                    // Use original question options as template
                    normalizedOptions = q.options?.map(o => ({ optionId: o.id, optionText: o.optionText })) ?? [];
                }
                return {
                    questionId: q.id,
                    questionText: entry?.questionText ?? '',
                    options: normalizedOptions,
                    followUpLabel: entry?.followUpLabel ?? '',
                    hasFollowUpText: q.hasFollowUpText,
                };
            });
            setTransQuestionsDraft(draft);
        } catch {
            setTransTemplateQuestions([]);
        }
    };

    const handleSaveTrans = async () => {
        setSaving(true);
        try {
            const payload: UpsertFormTemplateTranslationRequest = {
                ...transForm,
                questionsTranslation: transEditorMode === 'visual' ? JSON.stringify(transQuestionsDraft) : transForm.questionsTranslation ?? null,
            };
            await formTemplateService.upsertTranslationAsync(payload);
            setTransOpen(false);
            // Refresh translations in expanded row
            if (transTargetTemplate) {
                const trans = await formTemplateService.getTranslationsAsync(transTargetTemplate.id);
                setExpandedRows(r => ({
                    ...r,
                    [transTargetTemplate.id]: { ...r[transTargetTemplate.id], translations: trans },
                }));
            }
            await load();
        } catch {
            setError('Failed to save translation.');
        } finally { setSaving(false); }
    };

    // ─── Question CRUD ─────────────────────────────────────────────────────────

    const refreshDetail = async () => {
        if (!selectedTemplate) return;
        const t = await formTemplateService.getByIdAsync(selectedTemplate.id);
        setSelectedTemplate(t);
    };

    const handleAddQuestion = async () => {
        setSaving(true);
        try {
            await formTemplateService.addQuestionAsync(addQForm);
            setAddQOpen(false);
            setAddQForm(EMPTY_QUESTION);
            setOptionInput('');
            await refreshDetail();
        } catch {
            setError('Failed to add question.');
        } finally { setSaving(false); }
    };

    const handleUpdateQuestion = async () => {
        debugger
        setSaving(true);
        try {
            await formTemplateService.updateQuestionAsync(editQForm);
            setEditQOpen(false);
            setEditOptionInput('');
            await refreshDetail();
        } catch {
            setError('Failed to update question.');
        } finally { setSaving(false); }
    };

    const handleDeleteQuestion = async () => {
        if (!deleteQId) return;
        setSaving(true);
        try {
            await formTemplateService.deleteQuestionAsync(deleteQId);
            setDeleteQOpen(false);
            setDeleteQId(null);
            await refreshDetail();
        } catch {
            setError('Failed to delete question.');
        } finally { setSaving(false); }
    };

    const openEditQuestion = (q: FormQuestionResponse) => {
        setEditQForm({
            id: q.id, questionText: q.questionText, questionType: q.questionType,
            isRequired: q.isRequired, hasFollowUpText: q.hasFollowUpText,
            followUpLabel: q.followUpLabel, followUpTriggerOption: q.followUpTriggerOption,
            options: q.options.map(o => o.optionText),
        });
        setEditOptionInput('');
        setEditQOpen(true);
    };

    const needsOptions = (type: number) =>
        type === QuestionType.SingleChoice || type === QuestionType.MultipleChoice;

    return (
        <AdminLayout>
            <Box sx={{ p: 3 }}>
                {error && <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>{error}</Alert>}

                <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
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
                        <Button startIcon={<RefreshIcon />} onClick={load} variant="outlined">Refresh</Button>
                        <Button startIcon={<AddIcon />} onClick={() => setCreateOpen(true)} variant="contained">
                            New Template
                        </Button>
                    </Stack>
                </Stack>

                <Card>
                    <CardContent sx={{ p: 0 }}>
                        {loading ? (
                            <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                                <CircularProgress />
                            </Box>
                        ) : (
                            <TableContainer>
                                <Table>
                                    <TableHead>
                                        <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                                            <TableCell width={32}></TableCell>
                                            <TableCell>#</TableCell>
                                            <TableCell>Title</TableCell>
                                            {/* <TableCell align="center">Version</TableCell> */}
                                            <TableCell align="center">Logo Template</TableCell>
                                            <TableCell align="center">Status</TableCell>
                                            <TableCell>Last Updated</TableCell>
                                            <TableCell align="center">Actions</TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {filteredTemplates.length === 0 ? (
                                            <TableRow>
                                                <TableCell colSpan={7} align="center">
                                                    {searchQuery ? 'No templates match your search.' : 'No templates found.'}
                                                </TableCell>
                                            </TableRow>
                                        ) : paginatedTemplates.map((t, idx) => (
                                            <React.Fragment key={t.id}>
                                                {/* ─── Main row ─── */}
                                                <TableRow hover>
                                                    <TableCell>
                                                        <IconButton size="small" onClick={() => toggleExpand(t)}>
                                                            {expandedRows[t.id]?.open ? <ExpandLessIcon fontSize="small" /> : <ExpandMoreIcon fontSize="small" />}
                                                        </IconButton>
                                                    </TableCell>
                                                    <TableCell>{idx + 1}</TableCell>
                                                    <TableCell>{t.title}
                                                        <Chip label={`Version ${t.version}`} size="small" variant="outlined" color="primary" sx={{ m: 0.75 }} />
                                                        <Typography variant="caption" color="text.secondary" display="block">
                                                            {t.description ? t.description.substring(0, 80) + (t.description.length > 80 ? '…' : '') : '—'}
                                                        </Typography>
                                                    </TableCell>
                                                    {/* <TableCell align="center">
                                                        <Chip label={`v${t.version}`} size="small" color="primary" variant="outlined" />
                                                    </TableCell> */}
                                                    <TableCell align="center">
                                                        <img src={normalizeAttachmentUrl(t.logoUrl)} alt="Logo" style={{ maxHeight: 40, maxWidth: 80 }} />
                                                    </TableCell>
                                                    <TableCell align="center">
                                                        <Chip label={t.isActive ? 'Active' : 'Inactive'} size="small"
                                                            color={t.isActive ? 'success' : 'default'} />
                                                    </TableCell>
                                                    <TableCell>{FormatUtcTime.formatDateWithoutTime(t.updatedAt)} <br />by <strong>{t.updatedBy}</strong></TableCell>
                                                    <TableCell align="center">
                                                        <Stack direction="row" spacing={0.5} justifyContent="center">
                                                            <Tooltip title="View & Edit Questions">
                                                                <IconButton size="small" onClick={() => openDetail(t.id)}>
                                                                    <VisibilityIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>
                                                            <Tooltip title="Preview Form">
                                                                <IconButton size="small" color="info" onClick={() => openPreview(t.id)}>
                                                                    <PreviewIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>
                                                            <Tooltip title="Add / Edit Translation">
                                                                <IconButton size="small" color="secondary" onClick={() => toggleExpand(t)}>
                                                                    <TranslateIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>
                                                            <Tooltip title="Edit">
                                                                <IconButton size="small" onClick={() => openEdit(t)}>
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

                                                {/* ─── Expand row: translations ─── */}
                                                <TableRow>
                                                    <TableCell colSpan={8} sx={{ py: 0, borderBottom: expandedRows[t.id]?.open ? undefined : 'none' }}>
                                                        <Collapse in={expandedRows[t.id]?.open ?? false} timeout="auto" unmountOnExit>
                                                            <Box sx={{ py: 1.5, px: 4, bgcolor: 'grey.50' }}>
                                                                {/* ─── Tabs: Translations / History ─── */}
                                                                <Tabs value={expandedRows[t.id]?.tab ?? 0}
                                                                    onChange={(_, v) => setRowTab(t.id, v)}
                                                                    sx={{ mb: 1.5, minHeight: 32 }}>
                                                                    <Tab icon={<TranslateIcon />} iconPosition="start"
                                                                        label={`Translations (${expandedRows[t.id]?.translations.length ?? 0})`}
                                                                        sx={{ minHeight: 32, py: 0.5, textTransform: 'none', fontSize: 12 }} />
                                                                    <Tab icon={<HistoryIcon />} iconPosition="start"
                                                                        label={`Version History (${expandedRows[t.id]?.history.length ?? 0})`}
                                                                        sx={{ minHeight: 32, py: 0.5, textTransform: 'none', fontSize: 12 }} />
                                                                </Tabs>

                                                                {/* Translations tab */}
                                                                {(expandedRows[t.id]?.tab ?? 0) === 0 && (
                                                                    <>
                                                                        <Stack direction="row" justifyContent="flex-end" mb={1}>
                                                                            <Button size="small" startIcon={<AddIcon />} onClick={() => openAddTrans(t)}>
                                                                                Add Language
                                                                            </Button>
                                                                        </Stack>
                                                                        {(expandedRows[t.id]?.translations ?? []).length === 0 ? (
                                                                            <Typography variant="caption" color="text.disabled">
                                                                                No translations yet. Click "Add Language" to create one.
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
                                                                                        <TableCell>Footer Text</TableCell>
                                                                                        <TableCell>Questions</TableCell>
                                                                                        <TableCell>Last Updated</TableCell>
                                                                                        {/* <TableCell>Updated By</TableCell> */}
                                                                                        <TableCell align="right">Action</TableCell>
                                                                                    </TableRow>
                                                                                </TableHead>
                                                                                <TableBody>
                                                                                    {expandedRows[t.id]!.translations.map(tr => (
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
                                                                                                    {tr.footerText ? tr.footerText.substring(0, 35) + (tr.footerText.length > 35 ? '…' : '') : '—'}
                                                                                                </Typography>
                                                                                            </TableCell>
                                                                                            <TableCell>
                                                                                                <Chip size="small" variant="outlined"
                                                                                                    label={tr.questionsTranslation ? '✓ Provided' : '—'}
                                                                                                    color={tr.questionsTranslation ? 'success' : 'default'} />
                                                                                            </TableCell>
                                                                                            {/* <TableCell>{FormatUtcTime.formatDateTime(tr.updatedAt)}</TableCell> */}
                                                                                            <TableCell>
                                                                                                <Typography variant="caption">{FormatUtcTime.formatDateTime(tr.updatedAt)} <br />by <strong>{tr.updatedBy}</strong></Typography>
                                                                                            </TableCell>
                                                                                            <TableCell align="right">
                                                                                                <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                                                                                                    <Tooltip title="View Translation">
                                                                                                        <IconButton size="small" color="info"
                                                                                                            onClick={() => {
                                                                                                                setViewerPayload({ title: tr.title, languageCode: tr.languageCode, content: tr.questionsTranslation ? '' : '', description: tr.footerText ?? null, questionsTranslation: tr.questionsTranslation ?? null });
                                                                                                                setViewerOpen(true);
                                                                                                            }}>
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
                                                                {expandedRows[t.id]?.tab === 1 && (
                                                                    (expandedRows[t.id]?.history ?? []).length === 0 ? (
                                                                        <Typography variant="caption" color="text.disabled">No version history yet.</Typography>
                                                                    ) : (
                                                                        <Table size="small">
                                                                            <TableHead>
                                                                                <TableRow sx={{
                                                                                    bgcolor: `${muiTheme.palette.primary.main}`,
                                                                                    '& .MuiTableCell-root': { color: '#fff', fontWeight: 600, borderBottom: 'none' },
                                                                                }}>
                                                                                    <TableCell>Version</TableCell>
                                                                                    <TableCell>Title</TableCell>
                                                                                    <TableCell>Last Updated</TableCell>
                                                                                    {/* <TableCell>Changed By</TableCell> */}
                                                                                    <TableCell>Note</TableCell>
                                                                                </TableRow>
                                                                            </TableHead>
                                                                            <TableBody>
                                                                                {expandedRows[t.id]!.history.map(h => (
                                                                                    <TableRow key={h.id} hover>
                                                                                        <TableCell>
                                                                                            <Chip label={`v${h.version}`} size="small" color="primary" variant="outlined" />
                                                                                        </TableCell>
                                                                                        <TableCell>{h.title}</TableCell>
                                                                                        <TableCell>{FormatUtcTime.formatDateWithoutTime(h.updatedAt)} <br />by <strong>{h.updatedBy}</strong></TableCell>
                                                                                        {/* <TableCell><Typography variant="caption">{h.updatedBy}</Typography></TableCell> */}
                                                                                        <TableCell>
                                                                                            <Typography variant="caption" color="text.secondary">{h.changeNote ?? '—'}</Typography>
                                                                                        </TableCell>
                                                                                    </TableRow>
                                                                                ))}
                                                                            </TableBody>
                                                                        </Table>
                                                                    )
                                                                )}
                                                            </Box>
                                                        </Collapse>
                                                    </TableCell>
                                                </TableRow>
                                            </React.Fragment>
                                        ))}
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

                {/* ─── Create Template Dialog ─────────────────────────────────── */}
                <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>New Form Template</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Title" required fullWidth value={createForm.title}
                                onChange={e => setCreateForm(f => ({ ...f, title: e.target.value }))} />
                            <TextField label="Description" fullWidth multiline rows={3} value={createForm.description}
                                onChange={e => setCreateForm(f => ({ ...f, description: e.target.value }))} />
                            {/* <TextField label="Logo URL" fullWidth value={createForm.logoUrl}
                                onChange={e => setCreateForm(f => ({ ...f, logoUrl: e.target.value }))} /> */}

                            <FormControl fullWidth>
                                <Select
                                    value={createForm.logoUrl ?? ''}
                                    onChange={e => setCreateForm(f => ({ ...f, logoUrl: e.target.value }))}
                                >
                                    <MenuItem value="">None</MenuItem>
                                    {iconsOutlet.map(icon => (
                                        <MenuItem key={icon.id} value={icon.fileUrl}>
                                            <Stack direction="row" spacing={1} alignItems="center">
                                                <Box sx={{ width: 32, height: 32 }} >
                                                    <img src={getApiBase() + icon.fileUrl} alt={icon.name} style={{ width: 32, height: 32, marginRight: 8 }} />
                                                </Box>
                                                <Typography variant="body2">
                                                    {icon.name}
                                                </Typography>
                                            </Stack>
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>

                            <TextField label="Footer Text" fullWidth multiline rows={3} value={createForm.footerText}
                                onChange={e => setCreateForm(f => ({ ...f, footerText: e.target.value }))} />
                            <TextField label="Agreement Text" required fullWidth value={createForm.agreementText}
                                onChange={e => setCreateForm(f => ({ ...f, agreementText: e.target.value }))} />
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setCreateOpen(false)} variant="outlined">Cancel</Button>
                        <Button onClick={handleCreate} variant="contained" disabled={saving || !createForm.title || !createForm.agreementText}>
                            {saving ? <CircularProgress size={20} /> : 'Create'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Edit Template Dialog ───────────────────────────────────── */}
                <Dialog open={editOpen} onClose={() => setEditOpen(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>Edit Template</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Title" required fullWidth value={editForm.title}
                                onChange={e => setEditForm(f => ({ ...f, title: e.target.value }))} />
                            <TextField label="Description" fullWidth multiline rows={3} value={editForm.description}
                                onChange={e => setEditForm(f => ({ ...f, description: e.target.value }))} />
                            {/* <TextField label="Logo URL" fullWidth value={editForm.logoUrl}
                                onChange={e => setEditForm(f => ({ ...f, logoUrl: e.target.value }))} /> */}

                            <FormControl fullWidth>
                                <Select
                                    value={editForm.logoUrl ?? ''}
                                    onChange={e => setEditForm(f => ({ ...f, logoUrl: e.target.value }))}
                                >
                                    <MenuItem value="">None</MenuItem>
                                    {iconsOutlet.map(icon => (
                                        <MenuItem key={icon.id} value={icon.fileUrl}>
                                            <Stack direction="row" spacing={1} alignItems="center">
                                                <Box sx={{ width: 32, height: 32 }} >
                                                    <img src={getApiBase() + icon.fileUrl} alt={icon.name} style={{ width: 32, height: 32, marginRight: 8 }} />
                                                </Box>
                                                <Typography variant="body2">
                                                    {icon.name}
                                                </Typography>
                                            </Stack>
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>

                            <TextField label="Footer Text" fullWidth multiline rows={3} value={editForm.footerText}
                                onChange={e => setEditForm(f => ({ ...f, footerText: e.target.value }))} />
                            <TextField label="Agreement Text" required fullWidth value={editForm.agreementText}
                                onChange={e => setEditForm(f => ({ ...f, agreementText: e.target.value }))} />
                            <FormControlLabel control={
                                <Switch checked={editForm.isActive}
                                    onChange={e => setEditForm(f => ({ ...f, isActive: e.target.checked }))} />
                            } label="Active" />
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setEditOpen(false)} variant="outlined">Cancel</Button>
                        <Button onClick={handleUpdate} variant="contained" disabled={saving}>
                            {saving ? <CircularProgress size={20} /> : 'Save'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Delete Confirm ─────────────────────────────────────────── */}
                <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
                    <DialogTitle>Confirm Delete</DialogTitle>
                    <DialogContent>
                        <Typography>Are you sure you want to delete this template? This action cannot be undone.</Typography>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDeleteOpen(false)} variant="outlined">Cancel</Button>
                        <Button onClick={handleDelete} color="error" variant="contained" disabled={saving}>
                            {saving ? <CircularProgress size={20} /> : 'Delete'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Template Detail / Questions Dialog ─────────────────────── */}
                <Dialog open={detailOpen} onClose={() => setDetailOpen(false)} maxWidth="md" fullWidth>
                    <DialogTitle>
                        <Stack direction="row" justifyContent="space-between" alignItems="center">
                            <Box>
                                <Typography variant="h6">{selectedTemplate?.title}</Typography>
                                <Chip label={`v${selectedTemplate?.version}`} size="small" color="primary" variant="outlined" />
                            </Box>
                            <Button startIcon={<AddIcon />} variant="contained" size="small"
                                onClick={() => {
                                    setAddQForm({ ...EMPTY_QUESTION, formTemplateId: selectedTemplate?.id ?? 0 });
                                    setOptionInput('');
                                    setAddQOpen(true);
                                }}>
                                Add Question
                            </Button>
                        </Stack>
                    </DialogTitle>
                    <DialogContent dividers>
                        {(!selectedTemplate?.questions || selectedTemplate.questions.length === 0) ? (
                            <Typography color="text.secondary" textAlign="center" sx={{ py: 3 }}>
                                No questions yet. Click "Add Question" to get started.
                            </Typography>
                        ) : selectedTemplate.questions.map((q, idx) => (
                            <Accordion key={q.id} sx={{ mb: 1 }}>
                                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                                    <Stack direction="row" alignItems="center" spacing={1} sx={{ width: '100%', pr: 2 }}>
                                        <DragIcon color="disabled" fontSize="small" />
                                        <Typography sx={{ flexGrow: 1 }}>
                                            <strong>{idx + 1}.</strong> {q.questionText}
                                        </Typography>
                                        <Chip label={QUESTION_TYPE_LABELS[q.questionType] ?? q.questionType} size="small" />
                                        {q.isRequired && <Chip label="Required" size="small" color="warning" />}
                                        <IconButton size="small" onClick={e => { e.stopPropagation(); openEditQuestion(q); }}>
                                            <EditIcon fontSize="small" />
                                        </IconButton>
                                        <IconButton size="small" color="error"
                                            onClick={e => { e.stopPropagation(); setDeleteQId(q.id); setDeleteQOpen(true); }}>
                                            <DeleteIcon fontSize="small" />
                                        </IconButton>
                                    </Stack>
                                </AccordionSummary>
                                <AccordionDetails>
                                    {q.options.length > 0 && (
                                        <Stack spacing={0.5}>
                                            {q.options.map(o => (
                                                <Typography key={o.id} variant="body2" color="text.secondary">
                                                    • {o.optionText}
                                                </Typography>
                                            ))}
                                        </Stack>
                                    )}
                                    {q.hasFollowUpText && (
                                        <Typography variant="body2" color="info.main" sx={{ mt: 1 }}>
                                            Follow-up: "{q.followUpLabel}" (triggers on: {q.followUpTriggerOption ?? 'any'})
                                        </Typography>
                                    )}
                                </AccordionDetails>
                            </Accordion>
                        ))}
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDetailOpen(false)}>Close</Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Add Question Dialog ─────────────────────────────────────── */}
                <Dialog open={addQOpen} onClose={() => setAddQOpen(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>Add Question</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Question Text" required fullWidth multiline rows={2}
                                value={addQForm.questionText}
                                onChange={e => setAddQForm(f => ({ ...f, questionText: e.target.value }))} />
                            <FormControl fullWidth>
                                <InputLabel>Question Type</InputLabel>
                                <Select label="Question Type" value={addQForm.questionType}
                                    onChange={e => setAddQForm(f => ({ ...f, questionType: Number(e.target.value), options: [] }))}>
                                    {Object.entries(QUESTION_TYPE_LABELS).map(([k, v]) => (
                                        <MenuItem key={k} value={Number(k)}>{v}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <FormControlLabel control={
                                <Checkbox checked={addQForm.isRequired}
                                    onChange={e => setAddQForm(f => ({ ...f, isRequired: e.target.checked }))} />
                            } label="Required" />

                            {needsOptions(addQForm.questionType) && (
                                <Box>
                                    <Divider sx={{ mb: 1 }} />
                                    <Typography variant="subtitle2" sx={{ mb: 1 }}>Options</Typography>
                                    <Stack spacing={0.5} sx={{ mb: 1 }}>
                                        {addQForm.options.map((o, i) => (
                                            <Stack key={i} direction="row" alignItems="center" spacing={1}>
                                                <Typography variant="body2" sx={{ flexGrow: 1 }}>{i + 1}. {o}</Typography>
                                                <IconButton size="small" color="error"
                                                    onClick={() => setAddQForm(f => ({ ...f, options: f.options.filter((_, idx) => idx !== i) }))}>
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            </Stack>
                                        ))}
                                    </Stack>
                                    <Stack direction="row" spacing={1}>
                                        <TextField size="small" label="New option" value={optionInput}
                                            onChange={e => setOptionInput(e.target.value)}
                                            onKeyDown={e => {
                                                if (e.key === 'Enter' && optionInput.trim()) {
                                                    setAddQForm(f => ({ ...f, options: [...f.options, optionInput.trim()] }));
                                                    setOptionInput('');
                                                }
                                            }} sx={{ flexGrow: 1 }} />
                                        <Button variant="outlined" size="small"
                                            onClick={() => {
                                                if (optionInput.trim()) {
                                                    setAddQForm(f => ({ ...f, options: [...f.options, optionInput.trim()] }));
                                                    setOptionInput('');
                                                }
                                            }}>Add</Button>
                                    </Stack>
                                </Box>
                            )}

                            <FormControlLabel control={
                                <Checkbox checked={addQForm.hasFollowUpText}
                                    onChange={e => setAddQForm(f => ({ ...f, hasFollowUpText: e.target.checked }))} />
                            } label="Has follow-up text field" />
                            {addQForm.hasFollowUpText && (
                                <Stack spacing={1}>
                                    <TextField label="Follow-up Label" fullWidth value={addQForm.followUpLabel ?? ''}
                                        onChange={e => setAddQForm(f => ({ ...f, followUpLabel: e.target.value }))} />
                                    <TextField label="Trigger on answer" fullWidth value={addQForm.followUpTriggerOption ?? ''}
                                        onChange={e => setAddQForm(f => ({ ...f, followUpTriggerOption: e.target.value }))} />
                                </Stack>
                            )}
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setAddQOpen(false)} variant="outlined">Cancel</Button>
                        <Button onClick={handleAddQuestion} variant="contained"
                            disabled={saving || !addQForm.questionText}>
                            {saving ? <CircularProgress size={20} /> : 'Add Question'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Edit Question Dialog ────────────────────────────────────── */}
                <Dialog open={editQOpen} onClose={() => setEditQOpen(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>Edit Question</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Question Text" required fullWidth multiline rows={2}
                                value={editQForm.questionText}
                                onChange={e => setEditQForm(f => ({ ...f, questionText: e.target.value }))} />
                            <FormControl fullWidth>
                                <InputLabel>Question Type</InputLabel>
                                <Select label="Question Type" value={editQForm.questionType}
                                    onChange={e => setEditQForm(f => ({ ...f, questionType: Number(e.target.value), options: [] }))}>
                                    {Object.entries(QUESTION_TYPE_LABELS).map(([k, v]) => (
                                        <MenuItem key={k} value={Number(k)}>{v}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <FormControlLabel control={
                                <Checkbox checked={editQForm.isRequired}
                                    onChange={e => setEditQForm(f => ({ ...f, isRequired: e.target.checked }))} />
                            } label="Required" />

                            {needsOptions(editQForm.questionType) && (
                                <Box>
                                    <Divider sx={{ mb: 1 }} />
                                    <Typography variant="subtitle2" sx={{ mb: 1 }}>Options</Typography>
                                    <Stack spacing={0.5} sx={{ mb: 1 }}>
                                        {editQForm.options.map((o, i) => (
                                            <Stack key={i} direction="row" alignItems="center" spacing={1}>
                                                <Typography variant="body2" sx={{ flexGrow: 1 }}>{i + 1}. {o}</Typography>
                                                <IconButton size="small" color="error"
                                                    onClick={() => setEditQForm(f => ({ ...f, options: f.options.filter((_, idx) => idx !== i) }))}>
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            </Stack>
                                        ))}
                                    </Stack>
                                    <Stack direction="row" spacing={1}>
                                        <TextField size="small" label="New option" value={editOptionInput}
                                            onChange={e => setEditOptionInput(e.target.value)}
                                            onKeyDown={e => {
                                                if (e.key === 'Enter' && editOptionInput.trim()) {
                                                    setEditQForm(f => ({ ...f, options: [...f.options, editOptionInput.trim()] }));
                                                    setEditOptionInput('');
                                                }
                                            }} sx={{ flexGrow: 1 }} />
                                        <Button variant="outlined" size="small"
                                            onClick={() => {
                                                if (editOptionInput.trim()) {
                                                    setEditQForm(f => ({ ...f, options: [...f.options, editOptionInput.trim()] }));
                                                    setEditOptionInput('');
                                                }
                                            }}>Add</Button>
                                    </Stack>
                                </Box>
                            )}

                            <FormControlLabel control={
                                <Checkbox checked={editQForm.hasFollowUpText}
                                    onChange={e => setEditQForm(f => ({ ...f, hasFollowUpText: e.target.checked }))} />
                            } label="Has follow-up text field" />
                            {editQForm.hasFollowUpText && (
                                <Stack spacing={1}>
                                    <TextField label="Follow-up Label" fullWidth value={editQForm.followUpLabel ?? ''}
                                        onChange={e => setEditQForm(f => ({ ...f, followUpLabel: e.target.value }))} />
                                    <TextField label="Trigger on answer" fullWidth value={editQForm.followUpTriggerOption ?? ''}
                                        onChange={e => setEditQForm(f => ({ ...f, followUpTriggerOption: e.target.value }))} />
                                </Stack>
                            )}
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setEditQOpen(false)} variant="outlined">Cancel</Button>
                        <Button onClick={handleUpdateQuestion} variant="contained" disabled={saving}>
                            {saving ? <CircularProgress size={20} /> : 'Save'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Delete Question Confirm ─────────────────────────────────── */}
                <Dialog open={deleteQOpen} onClose={() => setDeleteQOpen(false)}>
                    <DialogTitle>Delete Question</DialogTitle>
                    <DialogContent>
                        <Typography>Are you sure you want to delete this question?</Typography>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDeleteQOpen(false)} variant="outlined">Cancel</Button>
                        <Button onClick={handleDeleteQuestion} color="error" variant="contained" disabled={saving}>
                            {saving ? <CircularProgress size={20} /> : 'Delete'}
                        </Button>
                    </DialogActions>
                </Dialog>



                {/* ─── Form Preview Dialog ─────────────────────────────────────────────── */}
                <Dialog open={previewOpen} onClose={() => setPreviewOpen(false)} maxWidth="md" fullWidth>
                    <DialogTitle>
                        <Stack direction="row" alignItems="center" spacing={1}>
                            <PreviewIcon color="primary" />
                            <Box>
                                <Typography fontWeight={700}>{previewTemplate?.title}</Typography>
                                <Typography variant="caption" color="text.secondary">
                                    Form Preview — v{previewTemplate?.version} · {previewTemplate?.questions?.length ?? 0} questions
                                </Typography>
                            </Box>
                        </Stack>
                    </DialogTitle>
                    <DialogContent dividers sx={{ p: 0 }}>
                        {previewTemplate && (
                            <Box sx={{
                                fontFamily: 'Arial, sans-serif', fontSize: 13,
                                p: 3, bgcolor: '#fff',
                            }}>
                                {/* Header */}
                                <Box sx={{ textAlign: 'center', mb: 3, pb: 2, borderBottom: '2px solid #333' }}>
                                    <Typography variant="h6" fontWeight={700} gutterBottom>
                                        {previewTemplate.title}
                                    </Typography>
                                    {previewTemplate.description && (
                                        <Typography variant="body2" color="text.secondary">
                                            {previewTemplate.description}
                                        </Typography>
                                    )}
                                </Box>

                                {/* Questions */}
                                <Stack spacing={2.5}>
                                    {(previewTemplate.questions ?? []).map((q, qi) => (
                                        <Box key={q.id} sx={{ p: 2, border: '1px solid #e0e0e0', borderRadius: 1 }}>
                                            {/* Question header */}
                                            <Stack direction="row" alignItems="flex-start" spacing={1} mb={1}>
                                                <Chip label={qi + 1} size="small" color="primary" sx={{ minWidth: 28, height: 22, fontSize: 11 }} />
                                                <Typography fontWeight={600} fontSize={13} sx={{ flex: 1 }}>
                                                    {q.questionText}
                                                    {q.isRequired && (
                                                        <Typography component="span" color="error" ml={0.5}>*</Typography>
                                                    )}
                                                </Typography>
                                                <Chip
                                                    label={QUESTION_TYPE_LABELS[q.questionType] ?? q.questionType}
                                                    size="small" variant="outlined"
                                                    sx={{ fontSize: 10, height: 20 }}
                                                />
                                            </Stack>

                                            {/* Answer area based on type */}
                                            {(q.questionType === QuestionType.SingleChoice || q.questionType === QuestionType.YesNo) && (
                                                <Stack spacing={0.5} pl={2}>
                                                    {(q.questionType === QuestionType.YesNo
                                                        ? [{ id: -1, optionText: 'Yes', sortOrder: 0 }, { id: -2, optionText: 'No', sortOrder: 1 }]
                                                        : q.options
                                                    ).map(opt => (
                                                        <Stack key={opt.id} direction="row" alignItems="center" spacing={1}>
                                                            <Box sx={{
                                                                width: 14, height: 14, borderRadius: '50%',
                                                                border: '2px solid #555', flexShrink: 0,
                                                            }} />
                                                            <Typography fontSize={12}>{opt.optionText}</Typography>
                                                        </Stack>
                                                    ))}
                                                </Stack>
                                            )}
                                            {q.questionType === QuestionType.MultipleChoice && (
                                                <Stack spacing={0.5} pl={2}>
                                                    {q.options.map(opt => (
                                                        <Stack key={opt.id} direction="row" alignItems="center" spacing={1}>
                                                            <Box sx={{
                                                                width: 14, height: 14, borderRadius: 0.5,
                                                                border: '2px solid #555', flexShrink: 0,
                                                            }} />
                                                            <Typography fontSize={12}>{opt.optionText}</Typography>
                                                        </Stack>
                                                    ))}
                                                </Stack>
                                            )}
                                            {q.questionType === QuestionType.TextInput && (
                                                <Box sx={{
                                                    mt: 0.5, ml: 2, height: 28, borderBottom: '1px solid #999',
                                                    bgcolor: '#fafafa',
                                                }} />
                                            )}
                                            {q.questionType === QuestionType.Date && (
                                                <Box sx={{ mt: 0.5, ml: 2 }}>
                                                    <Typography fontSize={11} color="text.disabled">
                                                        _____ / _____ / _________
                                                    </Typography>
                                                </Box>
                                            )}
                                            {q.questionType === QuestionType.Rating && (
                                                <Stack direction="row" spacing={1} pl={2} mt={0.5}>
                                                    {[1, 2, 3, 4, 5].map(n => (
                                                        <Box key={n} sx={{
                                                            width: 28, height: 28, border: '1px solid #999',
                                                            borderRadius: 0.5, display: 'flex', alignItems: 'center',
                                                            justifyContent: 'center', fontSize: 12, color: '#666',
                                                        }}>{n}</Box>
                                                    ))}
                                                </Stack>
                                            )}

                                            {/* Follow-up */}
                                            {q.hasFollowUpText && (
                                                <Box sx={{ mt: 1.5, ml: 2 }}>
                                                    <Typography fontSize={11} color="text.secondary" mb={0.5}>
                                                        {q.followUpLabel ?? 'Additional comments:'}
                                                    </Typography>
                                                    <Box sx={{ height: 24, borderBottom: '1px solid #999', bgcolor: '#fafafa' }} />
                                                </Box>
                                            )}
                                        </Box>
                                    ))}
                                </Stack>

                                {/* Footer */}
                                {previewTemplate.footerText && (
                                    <Box sx={{ mt: 3, pt: 2, borderTop: '1px solid #ccc', textAlign: 'center' }}>
                                        <Typography variant="caption" color="text.secondary">
                                            {previewTemplate.footerText}
                                        </Typography>
                                    </Box>
                                )}
                            </Box>
                        )}
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setPreviewOpen(false)}>Close</Button>
                        {previewTemplate && (
                            <Button variant="outlined" startIcon={<EditIcon />}
                                onClick={() => {
                                    setPreviewOpen(false);
                                    openDetail(previewTemplate.id);
                                }}>
                                Edit Questions
                            </Button>
                        )}
                    </DialogActions>
                </Dialog>

                {/* ─── Translation Upsert Dialog ─────────────────────────────────────── */}
                <Dialog open={transOpen} onClose={() => setTransOpen(false)} maxWidth="md" fullWidth>
                    <DialogTitle>
                        {editingTrans
                            ? `Edit Translation — ${editingTrans.languageCode.toUpperCase()} · ${transTargetTemplate?.title}`
                            : `Add Language Translation · ${transTargetTemplate?.title}`}
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
                                onChange={e => setTransForm(f => ({ ...f, description: e.target.value || null }))} />
                            <TextField label="Footer Text (optional)" fullWidth multiline rows={4}
                                value={transForm.footerText ?? ''}
                                onChange={e => setTransForm(f => ({ ...f, footerText: e.target.value || null }))} />
                            <TextField label="Agreement Text" fullWidth required
                                value={transForm.agreementText}
                                onChange={e => setTransForm(f => ({ ...f, agreementText: e.target.value }))} />
                            <Box>
                                <Tabs value={transEditorMode === 'visual' ? 0 : 1}
                                    onChange={(_, v) => {
                                        const newMode = v === 0 ? 'visual' : 'json';
                                        if (newMode === 'visual') {
                                            // try parse existing raw JSON into draft if present
                                            if (transForm.questionsTranslation) {
                                                try {
                                                    const parsed = JSON.parse(transForm.questionsTranslation);
                                                    if (Array.isArray(parsed)) setTransQuestionsDraft(parsed.map((p: any) => ({
                                                        questionId: p.id,
                                                        questionText: p.questionText ?? '',
                                                        followUpLabel: p.followUpLabel ?? '',
                                                        hasFollowUpText: p.hasFollowUpText ?? false, options: p.options ?? []
                                                    })));
                                                    setTransParseError(null);
                                                } catch (e: any) {
                                                    setTransParseError(e?.message ?? String(e));
                                                }
                                            }
                                        } else {
                                            // JSON mode: sync draft → raw text
                                            setTransForm(f => ({ ...f, questionsTranslation: JSON.stringify(transQuestionsDraft) || null }));
                                        }
                                        setTransEditorMode(newMode);
                                    }}
                                >
                                    <Tab label="Visual Editor" />
                                    <Tab label="JSON" />
                                </Tabs>
                                {transEditorMode === 'visual' ? (
                                    <Box sx={{ mt: 2, maxHeight: 400, overflowY: 'auto' }}>
                                        {transTemplateQuestions && transTemplateQuestions.length > 0 ? (
                                            <Box>
                                                {transTemplateQuestions.map(q => {
                                                    const draft = transQuestionsDraft.find(d => d.questionId === q.id);
                                                    // Always ensure we have a draft entry for each question
                                                    const currentDraft = draft ?? {
                                                        questionId: q.id,
                                                        questionText: '',
                                                        followUpLabel: '',
                                                        hasFollowUpText: q.hasFollowUpText ?? false,
                                                        options: q.options?.map((o: any) => ({ optionId: o.id, optionText: '' })) ?? []
                                                    };

                                                    return (
                                                        <Box key={q.id} sx={{ mb: 2, p: 1, border: '1px dashed', borderColor: 'divider', borderRadius: 1 }}>
                                                            <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
                                                                {q.sortOrder}. {q.questionText}
                                                                <Typography component="span" variant="caption" color="text.secondary" sx={{ ml: 1 }}>
                                                                    ({QUESTION_TYPE_LABELS[q.questionType]})
                                                                </Typography>
                                                            </Typography>
                                                            <TextField
                                                                label="Translated question"
                                                                fullWidth
                                                                multiline
                                                                rows={2}
                                                                value={currentDraft.questionText ?? ''}
                                                                onChange={e => {
                                                                    const newText = e.target.value;
                                                                    setTransQuestionsDraft(prev => {
                                                                        const exists = prev.find(p => p.questionId === q.id);
                                                                        if (exists) {
                                                                            return prev.map(p => p.questionId === q.id ? { ...p, questionText: newText } : p);
                                                                        } else {
                                                                            // Add new entry if not exists
                                                                            return [...prev, { ...currentDraft, questionText: newText }];
                                                                        }
                                                                    });
                                                                }}
                                                            />
                                                            {needsOptions(q.questionType) && (
                                                                <Box sx={{ mt: 1 }}>
                                                                    <Typography variant="caption">Options</Typography>
                                                                    <Box sx={{ mt: 0.5 }}>
                                                                        {(currentDraft.options ?? []).map((opt: any, i: number) => (
                                                                            <Box key={i} sx={{ display: 'flex', gap: 1, alignItems: 'center', mb: 0.5 }}>
                                                                                <Typography variant="caption" sx={{ minWidth: 80 }}>
                                                                                    Original: {q.options?.[i]?.optionText ?? '—'}
                                                                                </Typography>
                                                                                <TextField
                                                                                    size="small"
                                                                                    fullWidth
                                                                                    placeholder="Translated option"
                                                                                    value={opt?.optionText ?? ''}
                                                                                    onChange={e => {
                                                                                        const newOptText = e.target.value;
                                                                                        setTransQuestionsDraft(prev => {
                                                                                            const exists = prev.find(p => p.questionId === q.id);
                                                                                            if (exists) {
                                                                                                return prev.map(p => p.questionId === q.id
                                                                                                    ? { ...p, options: (p.options ?? []).map((o: any, oi: number) => oi === i ? { ...o, optionText: newOptText } : o) }
                                                                                                    : p
                                                                                                );
                                                                                            } else {
                                                                                                return [...prev, { ...currentDraft, options: currentDraft.options?.map((o: any, oi: number) => oi === i ? { ...o, optionText: newOptText } : o) ?? [] }];
                                                                                            }
                                                                                        });
                                                                                    }}
                                                                                />
                                                                            </Box>
                                                                        ))}
                                                                    </Box>
                                                                </Box>
                                                            )}
                                                            {q.hasFollowUpText && (
                                                                <Box sx={{ mt: 1 }}>
                                                                    <Typography variant="caption">Follow-up label</Typography>
                                                                    <TextField
                                                                        size="small"
                                                                        fullWidth
                                                                        placeholder="Translated follow-up label"
                                                                        value={currentDraft.followUpLabel ?? ''}
                                                                        onChange={e => {
                                                                            const newFollowUpLabel = e.target.value;
                                                                            setTransQuestionsDraft(prev => {
                                                                                const exists = prev.find(p => p.questionId === q.id);
                                                                                if (exists) {
                                                                                    return prev.map(p => p.questionId === q.id ? { ...p, followUpLabel: newFollowUpLabel } : p);
                                                                                } else {
                                                                                    // Add new entry if not exists
                                                                                    return [...prev, { ...currentDraft, followUpLabel: newFollowUpLabel }];
                                                                                }
                                                                            });
                                                                        }}
                                                                    />
                                                                </Box>
                                                            )}
                                                        </Box>
                                                    );
                                                })}
                                            </Box>
                                        ) : (
                                            <Typography variant="body2" color="text.secondary" align="center" sx={{ my: 2 }}>
                                                No questions available for this template.
                                            </Typography>
                                        )}
                                    </Box>
                                ) : (
                                    <Box sx={{ mt: 1 }}>
                                        <TextField
                                            label="Questions Translation (JSON)"
                                            fullWidth multiline rows={8}
                                            placeholder='[{"questionId": 1, "questionText": "...", "options": ["Option A", "Option B"]}]'
                                            value={transForm.questionsTranslation ?? ''}
                                            onChange={e => setTransForm(f => ({ ...f, questionsTranslation: e.target.value || null }))}
                                            inputProps={{ style: { fontFamily: 'monospace', fontSize: 12 } }}
                                            helperText={transParseError ? `JSON parse error: ${transParseError}` : 'Optional JSON — each entry maps a questionId to translated text and options'}
                                        />
                                        {(transForm.questionsTranslation) && (
                                            <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 1 }}>
                                                <Button size="small"
                                                    onClick={() => {
                                                        setViewerPayload({ title: `${transTargetTemplate?.title ?? ''} · ${transForm.languageCode.toUpperCase()}`, questionsTranslation: transForm.questionsTranslation ?? null });
                                                        setViewerOpen(true);
                                                    }}>
                                                    Preview JSON
                                                </Button>
                                            </Box>
                                        )}
                                    </Box>
                                )}
                            </Box>
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setTransOpen(false)} disabled={saving} variant="outlined">Cancel</Button>
                        <Button variant="contained" onClick={handleSaveTrans}
                            disabled={saving || !transForm.title || !transForm.agreementText || !!transParseError}>
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
                    questionsTranslation={viewerPayload?.questionsTranslation ?? null}
                />
            </Box>
        </AdminLayout>
    );
}

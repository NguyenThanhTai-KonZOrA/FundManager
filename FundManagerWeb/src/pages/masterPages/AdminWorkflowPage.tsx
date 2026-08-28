import React, { useState, useEffect, useCallback } from 'react';
import {
    Box, Card, CardContent, Typography, Button, Table, TableBody, TableCell,
    TableContainer, TableHead, TableRow, Chip, IconButton, Dialog, DialogTitle,
    DialogContent, DialogActions, TextField, Switch, FormControlLabel,
    CircularProgress, Alert, Tooltip, Stack, Select, MenuItem, FormControl,
    InputLabel, Paper, TablePagination, InputAdornment,
} from '@mui/material';
import { Autocomplete } from '@mui/material';
import {
    Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, Refresh as RefreshIcon,
    Visibility as VisibilityIcon, ArrowUpward as UpIcon, ArrowDownward as DownIcon,
    Search as SearchIcon,
} from '@mui/icons-material';
import AdminLayout from '../../components/layout/AdminLayout';
import { useSetPageTitle } from '../../hooks/useSetPageTitle';
import { PAGE_TITLES } from '../../constants/pageTitles';
import { workflowService, formTemplateService } from '../../services/formWorkflowService';
import { FormatUtcTime } from '../../utils/formatUtcTime';
import type {
    WorkflowResponse, WorkflowStepRequest, CreateWorkflowRequest, UpdateWorkflowRequest,
} from '../../types/formWorkflowType';
import { useAppData } from '../../contexts/AppDataContext';
import type { OutletResponse } from '../../types/outletType';
import { StepType } from '../../types/formWorkflowType';
import type { DocumentTemplateBriefResponse } from '../../types/documentTemplateType';
import { documentTemplateService } from '../../services/documentTemplateService';
import { useTheme } from '@mui/material/styles';
import type { FormTemplateBriefResponse } from '../../types/formTemplateType';
const STEP_TYPE_LABELS: Record<number, string> = {
    [StepType.FILLFORM]: 'Fill Form',
    [StepType.DOCUMENT_AND_SIGNATURE]: 'Document and Signature',
};

const EMPTY_STEP = (): WorkflowStepRequest => ({
    stepOrder: 1, stepType: StepType.FILLFORM, stepLabel: '', formTemplateId: null, documentTemplateId: null
});

export default function AdminWorkflowPage() {
    useSetPageTitle(PAGE_TITLES.WORKFLOW);
    const muiTheme = useTheme();
    const [workflows, setWorkflows] = useState<WorkflowResponse[]>([]);
    const [filteredWorkflows, setFilteredWorkflows] = useState<WorkflowResponse[]>([]);
    const [templates, setTemplates] = useState<FormTemplateBriefResponse[]>([]);
    const [documentTemplates, setDocumentTemplates] = useState<DocumentTemplateBriefResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Search & Pagination
    const [searchQuery, setSearchQuery] = useState('');
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);

    const [createOpen, setCreateOpen] = useState(false);
    const [editOpen, setEditOpen] = useState(false);
    const [detailOpen, setDetailOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);
    const [deleteId, setDeleteId] = useState<number | null>(null);
    const [saving, setSaving] = useState(false);
    const [selected, setSelected] = useState<WorkflowResponse | null>(null);

    const [createForm, setCreateForm] = useState<CreateWorkflowRequest>({
        name: '', description: '', outletId: 0, steps: [EMPTY_STEP()],
    });
    const [editForm, setEditForm] = useState<UpdateWorkflowRequest>({
        id: 0, name: '', description: '', isActive: true, outletId: 0, steps: [],
    });

    const { outlets } = useAppData();

    const load = useCallback(async () => {
        setLoading(true); setError(null);
        try {
            const [wf, tmpl, docTmpl] = await Promise.all([
                workflowService.getListAsync(),
                formTemplateService.getListAsync(),
                documentTemplateService.getListAsync(),
            ]);
            setWorkflows(wf);
            setFilteredWorkflows(wf);
            setTemplates(tmpl);
            setDocumentTemplates(docTmpl);
        } catch {
            setError('Failed to load workflows.');
        } finally { setLoading(false); }
    }, []);

    useEffect(() => { load(); }, [load]);

    // Filter workflows
    useEffect(() => {
        if (!searchQuery.trim()) {
            setFilteredWorkflows(workflows);
            setPage(0);
            return;
        }
        const query = searchQuery.toLowerCase();
        const filtered = workflows.filter(w =>
            w.name.toLowerCase().includes(query) ||
            w.description?.toLowerCase().includes(query) ||
            w.id.toString().includes(query)
        );
        setFilteredWorkflows(filtered);
        setPage(0);
    }, [searchQuery, workflows]);

    const paginatedWorkflows = filteredWorkflows.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

    // ─── Step helpers ──────────────────────────────────────────────────────────

    const updateCreateStep = (idx: number, patch: Partial<WorkflowStepRequest>) =>
        setCreateForm(f => ({
            ...f, steps: f.steps.map((s, i) => i === idx ? { ...s, ...patch } : s),
        }));

    const updateEditStep = (idx: number, patch: Partial<WorkflowStepRequest>) =>
        setEditForm(f => ({
            ...f, steps: f.steps.map((s, i) => i === idx ? { ...s, ...patch } : s),
        }));

    const moveStep = (
        steps: WorkflowStepRequest[], from: number, to: number
    ): WorkflowStepRequest[] => {
        const arr = [...steps];
        const [item] = arr.splice(from, 1);
        arr.splice(to, 0, item);
        return arr.map((s, i) => ({ ...s, stepOrder: i + 1 }));
    };

    // ─── CRUD ──────────────────────────────────────────────────────────────────

    const handleCreate = async () => {
        setSaving(true);
        try {
            await workflowService.createAsync({
                ...createForm,
                steps: createForm.steps.map((s, i) => ({ ...s, stepOrder: i + 1 })),
            });
            setCreateOpen(false);
            setCreateForm({ name: '', description: '', outletId: 0, steps: [EMPTY_STEP()] });
            await load();
        } catch { setError('Failed to create workflow.'); }
        finally { setSaving(false); }
    };

    const handleUpdate = async () => {
        setSaving(true);
        try {
            await workflowService.updateAsync({
                ...editForm,
                steps: editForm.steps.map((s, i) => ({ ...s, stepOrder: i + 1 })),
            });
            setEditOpen(false);
            await load();
        } catch { setError('Failed to update workflow.'); }
        finally { setSaving(false); }
    };

    const handleDelete = async () => {
        if (!deleteId) return;
        setSaving(true);
        try {
            await workflowService.deleteAsync(deleteId);
            setDeleteOpen(false);
            setDeleteId(null);
            await load();
        } catch { setError('Failed to delete workflow.'); }
        finally { setSaving(false); }
    };

    const openEdit = (w: WorkflowResponse) => {
        setEditForm({
            id: w.id, name: w.name, description: w.description, isActive: w.isActive, outletId: w.outletId,
            steps: w.steps.map(s => ({
                stepOrder: s.stepOrder,
                stepType: s.stepType,
                stepLabel: s.stepLabel,
                formTemplateId: s.formTemplateId,
                documentTemplateId: s.documentTemplateId,
            })),
        });
        setEditOpen(true);
    };

    // ─── Step row renderer ─────────────────────────────────────────────────────

    const renderStepRows = (
        steps: WorkflowStepRequest[],
        update: (idx: number, patch: Partial<WorkflowStepRequest>) => void,
        setSteps: (s: WorkflowStepRequest[]) => void
    ) => steps.map((step, idx) => (
        <Paper key={idx} variant="outlined" sx={{ p: 2, mb: 1 }}>
            <Stack spacing={1.5}>
                <Stack direction="row" alignItems="center" spacing={1}>
                    <Chip label={`Step ${idx + 1}`} size="small" />
                    <Box sx={{ flexGrow: 1 }} />
                    <IconButton size="small" disabled={idx === 0}
                        onClick={() => setSteps(moveStep(steps, idx, idx - 1))}>
                        <UpIcon fontSize="small" />
                    </IconButton>
                    <IconButton size="small" disabled={idx === steps.length - 1}
                        onClick={() => setSteps(moveStep(steps, idx, idx + 1))}>
                        <DownIcon fontSize="small" />
                    </IconButton>
                    <IconButton size="small" color="error"
                        onClick={() => setSteps(steps.filter((_, i) => i !== idx).map((s, i) => ({ ...s, stepOrder: i + 1 })))}>
                        <DeleteIcon fontSize="small" />
                    </IconButton>
                </Stack>
                <TextField label="Step Label" fullWidth required value={step.stepLabel}
                    onChange={e => update(idx, { stepLabel: e.target.value })} />
                <FormControl fullWidth>
                    <InputLabel>Step Type</InputLabel>
                    <Select label="Step Type" value={step.stepType as number}
                        onChange={e => update(idx, {
                            stepType: Number(e.target.value) as WorkflowStepRequest['stepType'], formTemplateId: null
                        })}>
                        {Object.entries(STEP_TYPE_LABELS).map(([k, v]) => (
                            <MenuItem key={k} value={k}>{v}</MenuItem>
                        ))}
                    </Select>
                </FormControl>
                {step.stepType === StepType.FILLFORM && (
                    <FormControl fullWidth>
                        <InputLabel>Form Template</InputLabel>
                        <Select label="Form Template" value={step.formTemplateId ?? ''}
                            onChange={e => update(idx, { formTemplateId: e.target.value as number })}>
                            <MenuItem value=""><em>None</em></MenuItem>
                            {templates.map(t => (
                                <MenuItem key={t.id} value={t.id}>{t.title} (v{t.version})</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                )}

                {step.stepType === StepType.DOCUMENT_AND_SIGNATURE && (
                    <FormControl fullWidth>
                        <InputLabel>Document Template</InputLabel>
                        <Select label="Document Template" value={step.documentTemplateId ?? ''}
                            onChange={e => update(idx, { documentTemplateId: e.target.value as number })}>
                            <MenuItem value=""><em>None</em></MenuItem>
                            {documentTemplates.map(t => (
                                <MenuItem key={t.id} value={t.id}>{t.title} (v{t.version})</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                )}
            </Stack>
        </Paper>
    ));

    return (
        <AdminLayout>
            <Box sx={{ p: 3 }}>
                {error && <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>{error}</Alert>}

                <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
                    <TextField
                        size="small"
                        placeholder="Search workflows..."
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
                    <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap', gap: 1, justifyContent: 'flex-end' }}>
                        <Button startIcon={<RefreshIcon />} onClick={load} variant="outlined">Refresh</Button>
                        <Button startIcon={<AddIcon />} onClick={() => setCreateOpen(true)} variant="contained">
                            New Workflow
                        </Button>
                    </Stack>
                </Stack>

                <Card>
                    <CardContent sx={{ p: 0 }}>
                        {loading ? (
                            <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>
                        ) : (
                            <TableContainer>
                                <Table size="medium" >
                                    <TableHead>
                                        <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                                            <TableCell>Name</TableCell>
                                            <TableCell>Outlet</TableCell>
                                            <TableCell align="center">Steps</TableCell>
                                            <TableCell align="center">Status</TableCell>
                                            <TableCell>Last Updated</TableCell>
                                            <TableCell align="center">Actions</TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {filteredWorkflows.length === 0 ? (
                                            <TableRow>
                                                <TableCell colSpan={6} align="center">
                                                    {searchQuery ? 'No workflows match your search.' : 'No workflows found.'}
                                                </TableCell>
                                            </TableRow>
                                        ) : paginatedWorkflows.map(w => (
                                            <TableRow key={w.id} hover>
                                                <TableCell>{w.name}</TableCell>
                                                <TableCell>{w.outletName || '—'}</TableCell>
                                                <TableCell align="center">
                                                    <Chip label={w.steps.length} size="small" />
                                                </TableCell>
                                                <TableCell align="center">
                                                    <Chip label={w.isActive ? 'Active' : 'Inactive'} size="small"
                                                        color={w.isActive ? 'success' : 'default'} />
                                                </TableCell>
                                                <TableCell>{FormatUtcTime.formatDateTime(w.updatedAt)}</TableCell>
                                                <TableCell align="center">
                                                    <Stack direction="row" spacing={0.5} justifyContent="center">
                                                        <Tooltip title="View">
                                                            <IconButton size="small" onClick={() => { setSelected(w); setDetailOpen(true); }}>
                                                                <VisibilityIcon fontSize="small" />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Edit">
                                                            <IconButton size="small" onClick={() => openEdit(w)}>
                                                                <EditIcon fontSize="small" />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Delete">
                                                            <IconButton size="small" color="error"
                                                                onClick={() => { setDeleteId(w.id); setDeleteOpen(true); }}>
                                                                <DeleteIcon fontSize="small" />
                                                            </IconButton>
                                                        </Tooltip>
                                                    </Stack>
                                                </TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        )}
                        {!loading && filteredWorkflows.length > 0 && (
                            <TablePagination
                                component="div"
                                count={filteredWorkflows.length}
                                page={page}
                                onPageChange={(_, newPage) => setPage(newPage)}
                                rowsPerPage={rowsPerPage}
                                onRowsPerPageChange={e => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0); }}
                                rowsPerPageOptions={[5, 10, 25, 50]}
                            />
                        )}
                    </CardContent>
                </Card>

                {/* ─── Create Dialog ──────────────────────────────────────────── */}
                <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>New Workflow</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Name" required fullWidth value={createForm.name}
                                onChange={e => setCreateForm(f => ({ ...f, name: e.target.value }))} />
                            <TextField label="Description" fullWidth multiline rows={2} value={createForm.description}
                                onChange={e => setCreateForm(f => ({ ...f, description: e.target.value }))} />
                            <Autocomplete
                                options={outlets}
                                getOptionLabel={(o: OutletResponse) => o.name}
                                value={outlets.find(o => o.id === createForm.outletId) || null}
                                onChange={(_, newValue) => setCreateForm(f => ({ ...f, outletId: newValue?.id ?? 0 }))}
                                renderInput={(params) => (
                                    <TextField {...params} label="Outlet" placeholder="Select outlet" />
                                )}
                                fullWidth
                            />
                            <Typography variant="subtitle2">Steps</Typography>
                            {renderStepRows(
                                createForm.steps,
                                updateCreateStep,
                                (s) => setCreateForm(f => ({ ...f, steps: s }))
                            )}
                            <Button startIcon={<AddIcon />} variant="outlined" size="small"
                                onClick={() => setCreateForm(f => ({
                                    ...f, steps: [...f.steps, { ...EMPTY_STEP(), stepOrder: f.steps.length + 1 }]
                                }))}>
                                Add Step
                            </Button>
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setCreateOpen(false)}>Cancel</Button>
                        <Button onClick={handleCreate} variant="contained" disabled={saving || !createForm.name}>
                            {saving ? <CircularProgress size={20} /> : 'Create'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Edit Dialog ─────────────────────────────────────────────── */}
                <Dialog open={editOpen} onClose={() => setEditOpen(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>Edit Workflow</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 1 }}>
                            <TextField label="Name" required fullWidth value={editForm.name}
                                onChange={e => setEditForm(f => ({ ...f, name: e.target.value }))} />
                            <TextField label="Description" fullWidth multiline rows={2} value={editForm.description}
                                onChange={e => setEditForm(f => ({ ...f, description: e.target.value }))} />
                            <Autocomplete
                                options={outlets}
                                getOptionLabel={(o: OutletResponse) => o.name}
                                value={outlets.find(o => o.id === editForm.outletId) || null}
                                onChange={(_, newValue) => setEditForm(f => ({ ...f, outletId: newValue?.id ?? 0 }))}
                                renderInput={(params) => (
                                    <TextField {...params} label="Outlet" placeholder="Select outlet" />
                                )}
                                fullWidth
                            />
                            <FormControlLabel control={
                                <Switch checked={editForm.isActive}
                                    onChange={e => setEditForm(f => ({ ...f, isActive: e.target.checked }))} />
                            } label="Active" />
                            <Typography variant="subtitle2">Steps</Typography>
                            {renderStepRows(
                                editForm.steps,
                                updateEditStep,
                                (s) => setEditForm(f => ({ ...f, steps: s }))
                            )}
                            <Button startIcon={<AddIcon />} variant="outlined" size="small"
                                onClick={() => setEditForm(f => ({
                                    ...f, steps: [...f.steps, { ...EMPTY_STEP(), stepOrder: f.steps.length + 1 }]
                                }))}>
                                Add Step
                            </Button>
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setEditOpen(false)}>Cancel</Button>
                        <Button onClick={handleUpdate} variant="contained" disabled={saving}>
                            {saving ? <CircularProgress size={20} /> : 'Save'}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Detail Dialog ────────────────────────────────────────────── */}
                <Dialog open={detailOpen} onClose={() => setDetailOpen(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>
                        {selected?.name}
                        {selected?.outletName && (
                            <Typography variant="caption" sx={{ ml: 1 }} color="text.secondary">
                                — {selected.outletName}
                            </Typography>
                        )}
                    </DialogTitle>
                    <DialogContent dividers>
                        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>{selected?.description}</Typography>
                        {selected?.steps.map(s => (
                            <Paper key={s.id} variant="outlined" sx={{ p: 1.5, mb: 1 }}>
                                <Stack direction="row" alignItems="center" spacing={1}>
                                    <Chip label={s.stepOrder} size="small" color="primary" />
                                    <Typography sx={{ flexGrow: 1 }}>{s.stepLabel}</Typography>
                                    <Chip label={STEP_TYPE_LABELS[s.stepType] ?? s.stepType} size="small" />
                                    {s.formTemplateTitle && (
                                        <Chip label={s.formTemplateTitle} size="small" variant="outlined" />
                                    )}
                                </Stack>
                            </Paper>
                        ))}
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDetailOpen(false)}>Close</Button>
                    </DialogActions>
                </Dialog>

                {/* ─── Delete Confirm ───────────────────────────────────────────── */}
                <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
                    <DialogTitle>Confirm Delete</DialogTitle>
                    <DialogContent>
                        <Typography>Are you sure you want to delete this workflow?</Typography>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
                        <Button onClick={handleDelete} color="error" variant="contained" disabled={saving}>
                            {saving ? <CircularProgress size={20} /> : 'Delete'}
                        </Button>
                    </DialogActions>
                </Dialog>
            </Box>
        </AdminLayout>
    );
}

import React, { useState, useEffect, useCallback } from 'react';
import {
    Box, Card, CardContent, Typography, Button, Table, TableBody, TableCell,
    TableContainer, TableHead, TableRow, Paper, Chip, IconButton, Dialog,
    DialogTitle, DialogContent, DialogActions, TextField, CircularProgress,
    Alert, Tooltip, Stack, Tabs, Tab, Switch, FormControlLabel,
    FormControl, Select, MenuItem, TablePagination, InputAdornment,
} from '@mui/material';
import {
    Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, Refresh as RefreshIcon,
    Search as SearchIcon,
} from '@mui/icons-material';
import AdminLayout from '../../components/layout/AdminLayout';
import { useSetPageTitle } from '../../hooks/useSetPageTitle';
import { PAGE_TITLES } from '../../constants/pageTitles';
import { languageService, patronTypeService } from '../../services/masterDataService';
import { FormatUtcTime } from '../../utils/formatUtcTime';
import type { LanguageResponse, CreateLanguageRequest, UpdateLanguageRequest } from '../../types/masterDataType';
import type { PatronTypeResponse, CreatePatronTypeRequest, UpdatePatronTypeRequest } from '../../types/masterDataType';
import { GREEN_TEAL } from '../../constants/ColorConstants';
import { useTheme } from '@mui/material/styles';
import { applicationImageService } from '../../services/applicationImageService';
import { ImageTypeEnum, type ApplicationImageResponse } from '../../types/applicationImageType';
import { getApiBase } from '../../utils/envConfig';
// ─── Language tab ──────────────────────────────────────────────────────────

const EMPTY_LANG: CreateLanguageRequest = { Code: '', Name: '', NativeName: '', FlagEmoji: '', SortOrder: 0 };

function LanguageTab() {
    const muiTheme = useTheme();
    const [rows, setRows] = useState<LanguageResponse[]>([]);
    const [filteredRows, setFilteredRows] = useState<LanguageResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [dialogOpen, setDialogOpen] = useState(false);
    const [editing, setEditing] = useState<LanguageResponse | null>(null);
    const [form, setForm] = useState<CreateLanguageRequest>(EMPTY_LANG);
    const [saving, setSaving] = useState(false);
    const [deleteId, setDeleteId] = useState<number | null>(null);
    const [flags, setFlags] = useState<ApplicationImageResponse[]>([]);

    // Search & Pagination
    const [searchQuery, setSearchQuery] = useState('');
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);

    const load = useCallback(async () => {
        setLoading(true); setError('');
        try {
            const data = await languageService.getAllAsync();
            setRows(data);
            setFilteredRows(data);
        }
        catch { setError('Failed to load languages.'); }
        finally { setLoading(false); }
    }, []);

    useEffect(() => { load(); }, [load]);

    // Filter based on search
    useEffect(() => {
        if (!searchQuery.trim()) {
            setFilteredRows(rows);
            setPage(0);
            return;
        }
        const query = searchQuery.toLowerCase();
        const filtered = rows.filter(r =>
            r.code.toLowerCase().includes(query) ||
            r.name.toLowerCase().includes(query) ||
            r.nativeName.toLowerCase().includes(query)
        );
        setFilteredRows(filtered);
        setPage(0);
    }, [searchQuery, rows]);

    const paginatedRows = filteredRows.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

    const loadFlags = useCallback(async () => {
        try {
            const flags = await applicationImageService.getByTypeAsync(ImageTypeEnum.Icon);
            setFlags(flags);
        } catch { setError('Failed to load flags.'); }
    }, [rows]);

    useEffect(() => { loadFlags(); }, [loadFlags]);

    const openCreate = () => { setEditing(null); setForm(EMPTY_LANG); setDialogOpen(true); };
    const openEdit = (r: LanguageResponse) => {
        setEditing(r);
        setForm({ Code: r.code, Name: r.name, NativeName: r.nativeName, FlagEmoji: r.flagEmoji ?? '', SortOrder: r.sortOrder });
        setDialogOpen(true);
    };

    const handleSave = async () => {
        setSaving(true);
        try {
            if (editing) await languageService.updateAsync({ ...form, Id: editing.id } as UpdateLanguageRequest);
            else await languageService.createAsync(form);
            setDialogOpen(false); load();
        } catch (e: any) { setError(e?.response?.data?.message ?? 'Save failed.'); }
        finally { setSaving(false); }
    };

    const handleDelete = async () => {
        if (!deleteId) return;
        try { await languageService.deleteAsync(deleteId); setDeleteId(null); load(); }
        catch { setError('Delete failed.'); }
    };

    const handleToggle = async (id: number) => {
        try { await languageService.toggleActiveAsync(id); load(); }
        catch { setError('Toggle failed.'); }
    };

    return (
        <Box>
            {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
            <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2} spacing={1}>
                <TextField
                    size="small"
                    placeholder="Search languages..."
                    value={searchQuery}
                    onChange={e => setSearchQuery(e.target.value)}
                    InputProps={{
                        startAdornment: (
                            <InputAdornment position="start">
                                <SearchIcon fontSize="small" />
                            </InputAdornment>
                        ),
                    }}
                    sx={{ width: 280 }}
                />
                <Stack direction="row" spacing={1}>
                    <Button startIcon={<RefreshIcon />} onClick={load} variant="outlined" size="small">Refresh</Button>
                    <Button startIcon={<AddIcon />} onClick={openCreate} variant="contained">Add Language</Button>
                </Stack>
            </Stack>
            <TableContainer component={Paper} variant="outlined">
                <Table>
                    <TableHead>
                        <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                            <TableCell>Code</TableCell><TableCell>Name</TableCell>
                            <TableCell>Native Name</TableCell><TableCell>Flag</TableCell>
                            <TableCell>Order</TableCell><TableCell>Status</TableCell>
                            <TableCell>Updated</TableCell><TableCell align="center">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {loading ? (
                            <TableRow><TableCell colSpan={8} align="center"><CircularProgress size={24} /></TableCell></TableRow>
                        ) : filteredRows.length === 0 ? (
                            <TableRow><TableCell colSpan={8} align="center">
                                {searchQuery ? 'No languages match your search.' : 'No languages found.'}
                            </TableCell></TableRow>
                        ) : paginatedRows.map(r => (
                            <TableRow key={r.id} hover>
                                <TableCell><Chip label={r.code} size="medium" variant="outlined" color="primary" /></TableCell>
                                <TableCell>{r.name}</TableCell>
                                <TableCell>{r.nativeName}</TableCell>
                                <TableCell>
                                    <img src={getApiBase() + r.flagEmoji} alt="flag" style={{ width: 32, height: 32 }} />
                                </TableCell>
                                <TableCell>{r.sortOrder}</TableCell>
                                <TableCell>
                                    <Switch checked={r.isActive} size="small" onChange={() => handleToggle(r.id)} />
                                </TableCell>
                                <TableCell>{FormatUtcTime.formatDateDDMMMYYYY(r.updatedAt)} <br />by <strong>{r.updatedBy}</strong></TableCell>
                                <TableCell align="center">
                                    <Tooltip title="Edit"><IconButton size="small" onClick={() => openEdit(r)}><EditIcon fontSize="small" /></IconButton></Tooltip>
                                    <Tooltip title="Delete"><IconButton size="small" color="error" onClick={() => setDeleteId(r.id)}><DeleteIcon fontSize="small" /></IconButton></Tooltip>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            {!loading && filteredRows.length > 0 && (
                <TablePagination
                    component="div"
                    count={filteredRows.length}
                    page={page}
                    onPageChange={(_, newPage) => setPage(newPage)}
                    rowsPerPage={rowsPerPage}
                    onRowsPerPageChange={e => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0); }}
                    rowsPerPageOptions={[5, 10, 25]}
                />
            )}

            {/* Create/Edit Dialog */}
            <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>{editing ? 'Edit Language' : 'Add Language'}</DialogTitle>
                <DialogContent>
                    <Stack spacing={2} mt={1}>
                        <TextField label="Code (e.g. en, vi, ko)" value={form.Code} onChange={e => setForm(f => ({ ...f, Code: e.target.value }))} fullWidth required inputProps={{ maxLength: 10 }} />
                        <TextField label="Name (English)" value={form.Name} onChange={e => setForm(f => ({ ...f, Name: e.target.value }))} fullWidth required />
                        <TextField label="Native Name" value={form.NativeName} onChange={e => setForm(f => ({ ...f, NativeName: e.target.value }))} fullWidth />
                        {/* <TextField label="Flag Icon (e.g. 🇺🇸, 🇻🇳, 🇰🇷)" value={form.FlagEmoji ?? ''} onChange={e => setForm(f => ({ ...f, FlagEmoji: e.target.value }))} fullWidth /> */}
                        <FormControl fullWidth>
                            <Select
                                value={form.FlagEmoji ?? ''}
                                onChange={e => setForm(f => ({ ...f, FlagEmoji: e.target.value }))}
                            >
                                <MenuItem value="">None</MenuItem>
                                {flags.map(flag => (
                                    <MenuItem key={flag.id} value={flag.fileUrl}>
                                        <Stack direction="row" spacing={1} alignItems="center">
                                            <Box sx={{ width: 32, height: 32 }} >
                                                <img src={getApiBase() + flag.fileUrl} alt={flag.name} style={{ width: 32, height: 32, marginRight: 8 }} />
                                            </Box>
                                            <Typography variant="body2">
                                                {flag.name}
                                            </Typography>
                                        </Stack>
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>

                        <TextField label="Sort Order" type="number" value={form.SortOrder} onChange={e => setForm(f => ({ ...f, SortOrder: Number(e.target.value) }))} fullWidth />
                    </Stack>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
                    <Button variant="contained" onClick={handleSave} disabled={saving || !form.Code || !form.Name} sx={{ bgcolor: GREEN_TEAL }}>
                        {saving ? <CircularProgress size={18} /> : 'Save'}
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Confirm delete */}
            <Dialog open={deleteId !== null} onClose={() => setDeleteId(null)}>
                <DialogTitle>Confirm Delete</DialogTitle>
                <DialogContent><Typography>Are you sure you want to delete this language?</Typography></DialogContent>
                <DialogActions>
                    <Button onClick={() => setDeleteId(null)}>Cancel</Button>
                    <Button color="error" variant="contained" onClick={handleDelete}>Delete</Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}

// ─── PatronType tab ────────────────────────────────────────────────────────

const EMPTY_PT: CreatePatronTypeRequest = { Name: '', ColorHex: '', Description: '', SortOrder: 0 };

function PatronTypeTab() {
    const muiTheme = useTheme();
    const [rows, setRows] = useState<PatronTypeResponse[]>([]);
    const [filteredRows, setFilteredRows] = useState<PatronTypeResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [dialogOpen, setDialogOpen] = useState(false);
    const [editing, setEditing] = useState<PatronTypeResponse | null>(null);
    const [form, setForm] = useState<CreatePatronTypeRequest>(EMPTY_PT);
    const [saving, setSaving] = useState(false);
    const [deleteId, setDeleteId] = useState<number | null>(null);

    // Search & Pagination
    const [searchQuery, setSearchQuery] = useState('');
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);

    const load = useCallback(async () => {
        setLoading(true); setError('');
        try {
            const data = await patronTypeService.getAllAsync();
            setRows(data);
            setFilteredRows(data);
        }
        catch { setError('Failed to load patron types.'); }
        finally { setLoading(false); }
    }, []);

    useEffect(() => { load(); }, [load]);

    // Filter based on search
    useEffect(() => {
        if (!searchQuery.trim()) {
            setFilteredRows(rows);
            setPage(0);
            return;
        }
        const query = searchQuery.toLowerCase();
        const filtered = rows.filter(r =>
            r.name.toLowerCase().includes(query) ||
            r.description?.toLowerCase().includes(query)
        );
        setFilteredRows(filtered);
        setPage(0);
    }, [searchQuery, rows]);

    const paginatedRows = filteredRows.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

    const openCreate = () => { setEditing(null); setForm(EMPTY_PT); setDialogOpen(true); };
    const openEdit = (r: PatronTypeResponse) => {
        setEditing(r);
        setForm({ Name: r.name, ColorHex: r.colorHex ?? '', Description: r.description ?? '', SortOrder: r.sortOrder });
        setDialogOpen(true);
    };

    const handleSave = async () => {
        setSaving(true);
        try {
            if (editing) await patronTypeService.updateAsync({ ...form, Id: editing.id } as UpdatePatronTypeRequest);
            else await patronTypeService.createAsync(form);
            setDialogOpen(false); load();
        } catch (e: any) { setError(e?.response?.data?.message ?? 'Save failed.'); }
        finally { setSaving(false); }
    };

    const handleDelete = async () => {
        if (!deleteId) return;
        try { await patronTypeService.deleteAsync(deleteId); setDeleteId(null); load(); }
        catch { setError('Delete failed.'); }
    };

    const handleToggle = async (id: number) => {
        try { await patronTypeService.toggleActiveAsync(id); load(); }
        catch { setError('Toggle failed.'); }
    };

    return (
        <Box>
            {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
            <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2} spacing={1}>
                <TextField
                    size="small"
                    placeholder="Search patron types..."
                    value={searchQuery}
                    onChange={e => setSearchQuery(e.target.value)}
                    InputProps={{
                        startAdornment: (
                            <InputAdornment position="start">
                                <SearchIcon fontSize="small" />
                            </InputAdornment>
                        ),
                    }}
                    sx={{ width: 280 }}
                />
                <Stack direction="row" spacing={1}>
                    <Button startIcon={<RefreshIcon />} onClick={load} variant="outlined" size="small">Refresh</Button>
                    <Button startIcon={<AddIcon />} onClick={openCreate} variant="contained">Add Patron Type</Button>
                </Stack>
            </Stack>
            <TableContainer component={Paper} variant="outlined">
                <Table>
                    <TableHead>
                        <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                            <TableCell>Name</TableCell><TableCell>Color</TableCell>
                            <TableCell>Description</TableCell><TableCell>Order</TableCell>
                            <TableCell>Status</TableCell><TableCell>Updated</TableCell>
                            <TableCell align="center">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {loading ? (
                            <TableRow><TableCell colSpan={7} align="center"><CircularProgress size={24} /></TableCell></TableRow>
                        ) : filteredRows.length === 0 ? (
                            <TableRow><TableCell colSpan={7} align="center">
                                {searchQuery ? 'No patron types match your search.' : 'No patron types found.'}
                            </TableCell></TableRow>
                        ) : paginatedRows.map(r => (
                            <TableRow key={r.id} hover>
                                <TableCell>{r.name}</TableCell>
                                <TableCell>
                                    {r.colorHex && (
                                        <Stack direction="row" spacing={1} alignItems="center">
                                            <Box sx={{ width: 16, height: 16, borderRadius: '50%', bgcolor: r.colorHex, border: '1px solid #ccc' }} />
                                            <Typography variant="caption">{r.colorHex}</Typography>
                                        </Stack>
                                    )}
                                </TableCell>
                                <TableCell>{r.description}</TableCell>
                                <TableCell>{r.sortOrder}</TableCell>
                                <TableCell>
                                    <Switch checked={r.isActive} size="small" onChange={() => handleToggle(r.id)} />
                                </TableCell>
                                <TableCell>{FormatUtcTime.formatDateDDMMMYYYY(r.updatedAt)} <br />by <strong>{r.updatedBy}</strong></TableCell>
                                <TableCell align="center">
                                    <Tooltip title="Edit"><IconButton size="small" onClick={() => openEdit(r)}><EditIcon fontSize="small" /></IconButton></Tooltip>
                                    <Tooltip title="Delete"><IconButton size="small" color="error" onClick={() => setDeleteId(r.id)}><DeleteIcon fontSize="small" /></IconButton></Tooltip>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            {!loading && filteredRows.length > 0 && (
                <TablePagination
                    component="div"
                    count={filteredRows.length}
                    page={page}
                    onPageChange={(_, newPage) => setPage(newPage)}
                    rowsPerPage={rowsPerPage}
                    onRowsPerPageChange={e => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0); }}
                    rowsPerPageOptions={[5, 10, 25]}
                />
            )}

            {/* Create/Edit Dialog */}
            <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>{editing ? 'Edit Patron Type' : 'Add Patron Type'}</DialogTitle>
                <DialogContent>
                    <Stack spacing={2} mt={1}>
                        <TextField label="Name" value={form.Name} onChange={e => setForm(f => ({ ...f, Name: e.target.value }))} fullWidth required />
                        <TextField label="Color Hex (e.g. #C0922E)" value={form.ColorHex ?? ''} onChange={e => setForm(f => ({ ...f, ColorHex: e.target.value }))} fullWidth />
                        <TextField label="Description" value={form.Description ?? ''} onChange={e => setForm(f => ({ ...f, Description: e.target.value }))} fullWidth multiline rows={2} />
                        <TextField label="Sort Order" type="number" value={form.SortOrder} onChange={e => setForm(f => ({ ...f, SortOrder: Number(e.target.value) }))} fullWidth />
                    </Stack>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
                    <Button variant="contained" onClick={handleSave} disabled={saving || !form.Name} sx={{ bgcolor: GREEN_TEAL }}>
                        {saving ? <CircularProgress size={18} /> : 'Save'}
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Confirm delete */}
            <Dialog open={deleteId !== null} onClose={() => setDeleteId(null)}>
                <DialogTitle>Confirm Delete</DialogTitle>
                <DialogContent><Typography>Are you sure you want to delete this patron type?</Typography></DialogContent>
                <DialogActions>
                    <Button onClick={() => setDeleteId(null)}>Cancel</Button>
                    <Button color="error" variant="contained" onClick={handleDelete}>Delete</Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}

// ─── Main Page ─────────────────────────────────────────────────────────────

export default function AdminLanguagePatronTypePage() {
    useSetPageTitle(PAGE_TITLES.MASTER_DATA);
    const [tab, setTab] = useState(0);

    return (
        <AdminLayout>
            <Box sx={{ p: 3 }}>
                {/* <Typography variant="h5" fontWeight={700} mb={3}>Master Data Management</Typography> */}
                <Card>
                    <CardContent>
                        <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 2, borderBottom: 1, borderColor: 'divider' }}>
                            <Tab label="Languages" />
                            <Tab label="Patron Types" />
                        </Tabs>
                        {tab === 0 && <LanguageTab />}
                        {tab === 1 && <PatronTypeTab />}
                    </CardContent>
                </Card>
            </Box>
        </AdminLayout>
    );
}

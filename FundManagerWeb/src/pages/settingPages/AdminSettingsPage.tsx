import {
    Box,
    Card,
    CardContent,
    Typography,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Button,
    IconButton,
    Chip,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    LinearProgress,
    Alert,
    Skeleton,
    Snackbar,
    Tooltip,
    Grid,
    Stack
} from "@mui/material";
import {
    Add as AddIcon,
    Edit as EditIcon,
    Refresh as RefreshIcon,
    Settings as SettingsIcon,
    ClearAll as ClearAllIcon
} from "@mui/icons-material";
import { useState, useEffect } from "react";
import AdminLayout from "../../components/layout/AdminLayout";
import { PAGE_TITLES } from "../../constants/pageTitles";
import { useSetPageTitle } from "../../hooks/useSetPageTitle";
import { FormatUtcTime } from "../../utils/formatUtcTime";
import { logError } from "../../utils/errorHandler";
import type { CreateSettingsRequest, SettingsInfoResponse, SettingsResponse, UpdateSettingsRequest } from "../../types/settingsType";
import { settingsService } from "../../services/systemControlService";
import { useTheme } from '@mui/material/styles';
import { useSnackbar } from "../../contexts/SnackbarContext";

interface SettingsFormData {
    key: string;
    value: string;
    description: string;
    category: string;
    dataType: string;
}

type ChipColor = 'default' | 'primary' | 'secondary' | 'success' | 'error' | 'info' | 'warning';

const DATA_TYPES = [
    { value: "String", label: "String" },
    { value: "Integer", label: "Integer" },
    { value: "Boolean", label: "Boolean" },
    { value: "Decimal", label: "Decimal" },
    { value: "JSON", label: "JSON" }
];

export default function AdminSettingsPage() {
    useSetPageTitle(PAGE_TITLES.SYSTEM_SETTINGS);
    const muiTheme = useTheme();

    const { showSnackbar } = useSnackbar();
    // States
    const [settings, setSettings] = useState<SettingsResponse[]>([]);
    const [settingsInfo, setSettingsInfo] = useState<SettingsInfoResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Dialog states
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const [selectedSetting, setSelectedSetting] = useState<SettingsResponse | null>(null);
    const [formData, setFormData] = useState<SettingsFormData>({
        key: '',
        value: '',
        description: '',
        category: '',
        dataType: 'String'
    });
    const [formLoading, setFormLoading] = useState(false);

    // Value viewer dialog
    const [valueDialogOpen, setValueDialogOpen] = useState(false);
    const [valueDialogContent, setValueDialogContent] = useState<{ key: string; value: string; dataType?: string } | null>(null);
    const [valueDialogLoading, setValueDialogLoading] = useState(false);
    const [valueDialogError, setValueDialogError] = useState<string | null>(null);

    // Load initial data
    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        try {
            setLoading(true);
            setError(null);

            const [settingsData, infoData] = await Promise.all([
                settingsService.getAllSettingsAsync(),
                settingsService.getSettingsInfoAsync()
            ]);

            setSettings(settingsData);
            setSettingsInfo(infoData);
        } catch (error) {
            logError('AdminSettingsPage.loadData', error);
            setError("Failed to load settings data");
        } finally {
            setLoading(false);
        }
    };

    // Handle Create New Setting
    const handleCreateClick = () => {
        setDialogMode('create');
        setSelectedSetting(null);
        setFormData({
            key: '',
            value: '',
            description: '',
            category: '',
            dataType: 'String'
        });
        setDialogOpen(true);
    };

    // Handle Update Setting
    const handleUpdateClick = async (setting: SettingsResponse) => {
        try {
            setFormLoading(true);
            const detailData = await settingsService.getSettingDetailAsync(setting.key);

            // Determine data type (prefer detailData, fall back to list item's dataType)
            const dt = detailData.dataType ?? setting.dataType ?? 'String';

            // If JSON, try to pretty-print for easier editing
            let formattedValue = detailData.value ?? '';
            if (dt && typeof formattedValue === 'string' && dt.toLowerCase() === 'json') {
                try {
                    const parsed = JSON.parse(formattedValue);
                    formattedValue = JSON.stringify(parsed, null, 2);
                } catch (err) {
                    // Leave raw if parsing fails
                }
            }

            setDialogMode('edit');
            setSelectedSetting(setting);
            setFormData({
                key: detailData.key,
                value: formattedValue,
                description: detailData.description,
                category: detailData.category,
                dataType: dt
            });
            setDialogOpen(true);
        } catch (error) {
            logError('AdminSettingsPage.handleUpdateClick', error);
            showSnackbar("Failed to load setting details", "error");
        } finally {
            setFormLoading(false);
        }
    };

    // Handle Clear Cache
    const handleClearCache = async (key: string) => {
        try {
            const result = await settingsService.clearCacheSettingAsync(key);
            showSnackbar(`Cache cleared: ${result.message}`, "success");
        } catch (error) {
            logError('AdminSettingsPage.handleClearCache', error);
            showSnackbar("Failed to clear cache", "error");
        }
    };

    // Handle Form Submit
    const handleFormSubmit = async () => {
        try {
            setFormLoading(true);

            if (dialogMode === 'create') {
                const createData: CreateSettingsRequest = {
                    key: formData.key,
                    value: formData.value,
                    description: formData.description,
                    category: formData.category,
                    dataType: formData.dataType
                };

                await settingsService.createSettingsAsync(createData);
                showSnackbar("Setting created successfully", "success");
            } else {
                const updateData: UpdateSettingsRequest = {
                    key: formData.key,
                    value: formData.value
                };

                const result = await settingsService.updateSettingAsync(selectedSetting!.key, updateData);

                if (result.requiresRestart) {
                    showSnackbar(`Setting updated. ${result.warning || 'Restart may be required.'}`, "warning");
                } else {
                    showSnackbar("Setting updated successfully", "success");
                }
            }

            setDialogOpen(false);
            loadData(); // Reload data
        } catch (error) {
            logError('AdminSettingsPage.handleFormSubmit', error);
            showSnackbar(`Failed to ${dialogMode === 'create' ? 'create' : 'update'} setting`, "error");
        } finally {
            setFormLoading(false);
        }
    };

    // Open value viewer dialog for long values or JSON
    const handleOpenValueDialog = (setting: SettingsResponse) => {
        let display = setting.value ?? '';
        const dt = setting.dataType ?? '';
        if (dt && dt.toLowerCase() === 'json' && typeof display === 'string') {
            try {
                const parsed = JSON.parse(display);
                display = JSON.stringify(parsed, null, 2);
            } catch (err) {
                // leave as-is
            }
        }

        setValueDialogContent({ key: setting.key, value: display, dataType: dt });
        setValueDialogOpen(true);
    };

    const handleCloseValueDialog = () => {
        setValueDialogOpen(false);
        setValueDialogContent(null);
    };

    const handleUpdateValue = async () => {
        if (!valueDialogContent) return;
        try {
            setValueDialogLoading(true);
            setValueDialogError(null);

            const { key, value, dataType } = valueDialogContent;

            // If JSON type, validate JSON first
            if (dataType && dataType.toLowerCase() === 'json') {
                try {
                    JSON.parse(value ?? '');
                } catch (err) {
                    setValueDialogError('Invalid JSON. Please fix syntax before updating.');
                    showSnackbar('Invalid JSON. Please fix syntax.', 'error');
                    return;
                }
            }

            const updateData: UpdateSettingsRequest = { key, value: value ?? '' };
            const result = await settingsService.updateSettingAsync(key, updateData);

            if (result.requiresRestart) {
                showSnackbar(`Setting updated. ${result.warning || 'Restart may be required.'}`, 'warning');
            } else {
                showSnackbar('Setting updated successfully', 'success');
            }

            setValueDialogOpen(false);
            setValueDialogContent(null);
            loadData();
        } catch (err) {
            logError('AdminSettingsPage.handleUpdateValue', err);
            showSnackbar('Failed to update setting', 'error');
        } finally {
            setValueDialogLoading(false);
        }
    };

    const getCategoryColor = (category: string): ChipColor => {
        const colors: Record<string, ChipColor> = {
            'System': 'success',
            'Integration': 'info',
            'Performance': 'warning',
            'Security': 'error',
            'Business': 'success',
            'URLs': 'secondary',
            'SLA': 'warning'
        };
        return colors[category] || 'default';
    };

    return (
        <AdminLayout>
            <Box>
                {/* Header */}
                <Box sx={{ mb: 3 }}>
                    <Typography variant="body1" color="text.secondary">
                        Manage application configuration settings
                    </Typography>
                </Box>

                {/* Info Cards */}
                {settingsInfo && (
                    <Grid container spacing={2} sx={{ mb: 3 }}>
                        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                            <Card>
                                <CardContent>
                                    <Typography color="text.secondary" gutterBottom>
                                        Cache Expiration
                                    </Typography>
                                    <Typography variant="h5">
                                        {settingsInfo.cacheExpirationMinutes} min
                                    </Typography>
                                </CardContent>
                            </Card>
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                            <Card>
                                <CardContent>
                                    <Typography color="text.secondary" gutterBottom>
                                        Total Settings
                                    </Typography>
                                    <Typography variant="h5">
                                        {settings.length}
                                    </Typography>
                                </CardContent>
                            </Card>
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                            <Card>
                                <CardContent>
                                    <Typography color="text.secondary" gutterBottom>
                                        Categories
                                    </Typography>
                                    <Typography variant="h5">
                                        {settingsInfo.categories?.length || 0}
                                    </Typography>
                                </CardContent>
                            </Card>
                        </Grid>
                    </Grid>
                )}

                {/* Actions */}
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={handleCreateClick}
                        disabled={loading}
                    >
                        Create New Setting
                    </Button>

                    <Button
                        variant="outlined"
                        startIcon={<RefreshIcon />}
                        onClick={loadData}
                        disabled={loading}
                    >
                        Refresh
                    </Button>
                </Box>

                {loading && <LinearProgress sx={{ mb: 2 }} />}

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                {/* Settings Table */}
                <Card>
                    <CardContent sx={{ p: 0 }}>
                        <TableContainer component={Paper} variant="outlined">
                            <Table>
                                <TableHead>
                                    <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                                        <TableCell sx={{
                                            fontWeight: 600,
                                            textAlign: 'center',
                                            position: 'sticky',
                                            left: 0,
                                            backgroundColor: muiTheme.palette.background.paper,
                                            zIndex: 3,
                                            boxShadow: '2px 0 5px rgba(0,0,0,0.1)',
                                            bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 }
                                        }}>
                                            Actions
                                        </TableCell>
                                        <TableCell>Key</TableCell>
                                        <TableCell>Value</TableCell>
                                        <TableCell>Data Type</TableCell>
                                        <TableCell>Category</TableCell>
                                        <TableCell>Description</TableCell>
                                        <TableCell>Status</TableCell>
                                        <TableCell>Updated</TableCell>

                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {loading ? (
                                        Array.from({ length: 7 }).map((_, index) => (
                                            <TableRow key={index}>
                                                <TableCell><Skeleton width="120px" /></TableCell>
                                                <TableCell><Skeleton width="100px" /></TableCell>
                                                <TableCell><Skeleton width="200px" /></TableCell>
                                                <TableCell><Skeleton width="100px" /></TableCell>
                                                <TableCell><Skeleton width="100px" /></TableCell>
                                                <TableCell><Skeleton width="200px" /></TableCell>
                                                <TableCell><Skeleton width="120px" /></TableCell>
                                            </TableRow>
                                        ))
                                    ) : settings.length > 0 ? (
                                        settings.map((setting) => (
                                            <TableRow
                                                key={setting.id}
                                                sx={{ '&:nth-of-type(even)': { bgcolor: 'grey.25' } }}
                                            >
                                                <TableCell
                                                    sx={{
                                                        fontWeight: 600,
                                                        textAlign: 'center',
                                                        position: 'sticky',
                                                        left: 0,
                                                        backgroundColor: 'background.paper',
                                                        zIndex: 3,
                                                        boxShadow: '2px 0 5px rgba(0,0,0,0.1)'
                                                    }}>
                                                    <Stack direction="row" spacing={1} justifyContent="center">
                                                        <Tooltip title="Update Setting">
                                                            <IconButton
                                                                size="small"
                                                                onClick={() => handleUpdateClick(setting)}
                                                                disabled={formLoading}
                                                            >
                                                                <EditIcon fontSize="small" />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title={`Clear Cache: AppSetting_${setting.key}`}>
                                                            <IconButton
                                                                size="small"
                                                                onClick={() => handleClearCache(setting.key)}
                                                                color="warning"
                                                            >
                                                                <ClearAllIcon fontSize="small" />
                                                            </IconButton>
                                                        </Tooltip>
                                                    </Stack>
                                                </TableCell>
                                                <TableCell sx={{ fontFamily: 'monospace', fontWeight: 500 }}>
                                                    {setting.key}
                                                </TableCell>
                                                <TableCell>
                                                    {((setting.value ?? '').length > 200 || (setting.dataType ?? '').toLowerCase() === 'json') ? (
                                                        <Tooltip title="Click to view full value">
                                                            <Box
                                                                onClick={() => handleOpenValueDialog(setting)}
                                                                sx={{
                                                                    maxWidth: 200,
                                                                    overflow: 'hidden',
                                                                    textOverflow: 'ellipsis',
                                                                    cursor: 'pointer',
                                                                    color: 'primary.main',
                                                                    whiteSpace: 'nowrap',
                                                                    '&:hover': {
                                                                        textDecoration: 'underline'
                                                                    },
                                                                }}
                                                                role="button"
                                                            >
                                                                {setting.value}
                                                            </Box>
                                                        </Tooltip>
                                                    ) : (
                                                        <Box sx={{ maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                                                            {setting.value}
                                                        </Box>
                                                    )}
                                                </TableCell>
                                                <TableCell sx={{ fontFamily: 'monospace', fontWeight: 500 }}>
                                                    {setting.dataType}
                                                </TableCell>
                                                <TableCell>
                                                    <Chip
                                                        label={setting.category}
                                                        size="small"
                                                        color={getCategoryColor(setting.category)}
                                                        variant="outlined"
                                                    />
                                                </TableCell>
                                                <TableCell>
                                                    <Box sx={{ maxWidth: 300, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                                                        {setting.description}
                                                    </Box>
                                                </TableCell>

                                                <TableCell>
                                                    <Chip
                                                        label={setting.isActive ? "Active" : "Inactive"}
                                                        size="small"
                                                        color={setting.isActive ? "success" : "default"}
                                                        variant="filled"
                                                    />
                                                </TableCell>
                                                <TableCell>
                                                    <Typography variant="caption" color="text.secondary">
                                                        {FormatUtcTime.formatDateTime(setting.updatedAt)}
                                                    </Typography>
                                                    <br />
                                                    <Typography variant="caption" color="text.secondary"> <br />
                                                        by {setting.updatedBy}
                                                    </Typography>
                                                </TableCell>
                                            </TableRow>
                                        ))
                                    ) : (
                                        <TableRow>
                                            <TableCell colSpan={7} sx={{ textAlign: 'center', py: 4 }}>
                                                <Typography color="text.secondary">
                                                    No settings found
                                                </Typography>
                                            </TableCell>
                                        </TableRow>
                                    )}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </CardContent>
                </Card>
            </Box>

            {/* Value Viewer Dialog */}
            <Dialog
                open={valueDialogOpen}
                onClose={handleCloseValueDialog}
                maxWidth="lg"
                fullWidth
            >
                <DialogTitle>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        Value - {valueDialogContent?.key}
                        {valueDialogContent?.dataType ? ` (${valueDialogContent.dataType})` : ''}
                    </Box>
                </DialogTitle>
                <DialogContent dividers>
                    <TextField
                        value={valueDialogContent?.value ?? ''}
                        onChange={(e) => setValueDialogContent(prev => prev ? { ...prev, value: e.target.value } : prev)}
                        multiline
                        fullWidth
                        minRows={10}
                        InputProps={{ readOnly: !(valueDialogContent?.dataType?.toLowerCase() === 'json') }}
                        sx={{ fontFamily: 'monospace' }}
                    />

                    {valueDialogError && (
                        <Box sx={{ mt: 1 }}>
                            <Alert severity="error">{valueDialogError}</Alert>
                        </Box>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseValueDialog} disabled={valueDialogLoading}>Close</Button>
                    {valueDialogContent?.dataType?.toLowerCase() === 'json' && (
                        <Button onClick={handleUpdateValue} variant="contained" disabled={valueDialogLoading}>
                            {valueDialogLoading ? 'Updating...' : 'Update'}
                        </Button>
                    )}
                </DialogActions>
            </Dialog>

            {/* Create/Update Dialog */}
            <Dialog
                open={dialogOpen}
                onClose={() => setDialogOpen(false)}
                maxWidth="sm"
                fullWidth
            >
                <DialogTitle>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <SettingsIcon />
                        {dialogMode === 'create' ? 'Create New Setting' : 'Update Setting'}
                    </Box>
                </DialogTitle>
                <DialogContent>
                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
                        <TextField
                            label="Key"
                            value={formData.key}
                            onChange={(e) => setFormData({ ...formData, key: e.target.value })}
                            disabled={dialogMode === 'edit' || formLoading}
                            required
                            fullWidth
                        />

                        <TextField
                            label="Value"
                            value={formData.value}
                            onChange={(e) => setFormData({ ...formData, value: e.target.value })}
                            disabled={formLoading}
                            required
                            fullWidth
                            multiline
                            rows={3}
                        />

                        <TextField
                            label="Description"
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                            disabled={dialogMode === 'edit' || formLoading}
                            fullWidth
                            multiline
                            rows={2}
                        />

                        <FormControl fullWidth disabled={dialogMode === 'edit' || formLoading}>
                            <InputLabel>Category</InputLabel>
                            <Select
                                value={formData.category}
                                label="Category"
                                onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                            >
                                {settingsInfo?.categories?.map((cat) => (
                                    <MenuItem key={cat.name} value={cat.name}>
                                        {cat.name} - {cat.description}
                                    </MenuItem>
                                )) || []}
                            </Select>
                        </FormControl>

                        {dialogMode === 'create' && (
                            <FormControl fullWidth disabled={formLoading}>
                                <InputLabel>Data Type</InputLabel>
                                <Select
                                    value={formData.dataType}
                                    label="Data Type"
                                    onChange={(e) => setFormData({ ...formData, dataType: e.target.value })}
                                >
                                    {DATA_TYPES.map((type) => (
                                        <MenuItem key={type.value} value={type.value}>
                                            {type.label}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        )}
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDialogOpen(false)} disabled={formLoading}>
                        Cancel
                    </Button>
                    <Button
                        onClick={handleFormSubmit}
                        variant="contained"
                        disabled={formLoading || !formData.key || !formData.value}
                    >
                        {formLoading ? 'Saving...' : (dialogMode === 'create' ? 'Create' : 'Update')}
                    </Button>
                </DialogActions>
            </Dialog>
        </AdminLayout>
    );
}
import React, { useState, useEffect, useCallback } from 'react';
import {
    Box,
    Card,
    CardContent,
    Typography,
    Button,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TablePagination,
    Paper,
    IconButton,
    Tooltip,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Stack,
    Chip,
    Alert,
    CircularProgress,
    Autocomplete,
    ListItemSecondaryAction,
    ListItem,
    List,
    ListItemText
} from '@mui/material';
import {
    Add as AddIcon,
    Edit as EditIcon,
    Delete as DeleteIcon,
    Refresh as RefreshIcon,
    DevicesOther as DevicesIcon,
    CheckCircle as CheckCircleIcon,
    Computer as ComputerIcon,
    ContentCopy as ContentCopyIcon,
    Search,
} from '@mui/icons-material';
import { mappingDeviceService, staffDeviceService } from '../../services/deviceMappingService';
import type {
    MappingDataResponse,
    CreateMappingRequest,
    UpdateMappingRequest,
    GetMappingByStaffDeviceResponse,
    StaffDeviceResponse,
    PatronDeviceResponse,
    CurrentHostNameResponse
} from '../../types/deviceType';
import { useAppData } from '../../contexts/AppDataContext';
import type { OutletResponse } from '../../types/outletType';
import AdminLayout from '../../components/layout/AdminLayout';
import { useSetPageTitle } from '../../hooks/useSetPageTitle';
import { PAGE_TITLES } from '../../constants/pageTitles';
import Swal from 'sweetalert2';
import { FormatUtcTime } from '../../utils/formatUtcTime';
import { extractErrorMessage, logError } from '../../utils/errorHandler';
import { useTheme } from '@mui/material/styles';
import { useSnackbar } from '../../contexts/SnackbarContext';
const DeviceMappingSettingsPage: React.FC = () => {
    const muiTheme = useTheme();
    useSetPageTitle(PAGE_TITLES.DEVICE_MAPPING);

    const { showSnackbar } = useSnackbar();
    // States
    const [mappings, setMappings] = useState<MappingDataResponse[]>([]);
    const [filteredMappings, setFilteredMappings] = useState<MappingDataResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [createDialogOpen, setCreateDialogOpen] = useState(false);
    const [updateDialogOpen, setUpdateDialogOpen] = useState(false);
    const [currentDeviceMapping, setCurrentDeviceMapping] = useState<GetMappingByStaffDeviceResponse | null>(null);
    const [staffDevices, setStaffDevices] = useState<StaffDeviceResponse[]>([]);
    const [patronDevices, setPatronDevices] = useState<PatronDeviceResponse[]>([]);
    const [loadingDevices, setLoadingDevices] = useState(false);
    const [hostnameData, setHostnameData] = useState<CurrentHostNameResponse | null>(null);
    const [hostnameDialogOpen, setHostnameDialogOpen] = useState(false);
    const [outletId, setOutletId] = useState<number | null>(null);
    const { outlets } = useAppData();

    // Pagination states
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);

    // Search state
    const [searchQuery, setSearchQuery] = useState('');
    // Form states for Create
    const [createForm, setCreateForm] = useState<CreateMappingRequest>({
        StaffDeviceName: '',
        PatronDeviceName: '',
        OutletId: 0,
        Notes: ''
    });

    // Form states for Update
    const [updateForm, setUpdateForm] = useState<UpdateMappingRequest>({
        Id: 0,
        NewStaffDeviceName: '',
        NewPatronDeviceName: '',
        Location: '',
        Notes: '',
        OutletId: 0,
        OutletName: ''
    });

    // Snackbar
    const [snackbar, setSnackbar] = useState<{
        open: boolean;
        message: string;
        severity: 'success' | 'error' | 'info' | 'warning';
    }>({
        open: false,
        message: '',
        severity: 'info'
    });

    // Load all mappings
    const loadMappings = useCallback(async () => {
        try {
            setLoading(true);
            const response = await mappingDeviceService.getAllMappingsAsync();
            setMappings(response);
            setFilteredMappings(response);
            showSnackbar(`Loaded ${response.length} mappings`, 'success');
        } catch (err) {
            console.error('Error loading mappings:', err);
            showSnackbar('Failed to load mappings', 'error');
        } finally {
            setLoading(false);
        }
    }, []);

    // Filter mappings based on search query
    useEffect(() => {
        if (!searchQuery.trim()) {
            setFilteredMappings(mappings);
            setPage(0);
            return;
        }

        const query = searchQuery.toLowerCase();
        const filtered = mappings.filter(mapping =>
            mapping.staffDeviceName.toLowerCase().includes(query) ||
            mapping.patronDeviceName.toLowerCase().includes(query) ||
            mapping.location.toLowerCase().includes(query) ||
            (mapping.notes && mapping.notes.toLowerCase().includes(query)) ||
            mapping.id.toString().includes(query) ||
            mapping.outletId.toString().includes(query) ||
            mapping.propertyName.toLowerCase().includes(query)
        );
        setFilteredMappings(filtered);
        setPage(0);
    }, [searchQuery, mappings]);

    // Load on mount
    useEffect(() => {
        loadMappings();
    }, [loadMappings]);

    // Load staff and patron devices
    const loadDevices = async () => {
        try {
            setLoadingDevices(true);
            const response = await mappingDeviceService.getStaffAndPatronDevicesAsync();
            setStaffDevices(response.staffDevices);
            setPatronDevices(response.patronDevices);
        } catch (err) {
            console.error('Error loading devices:', err);
            showSnackbar('Failed to load devices', 'error');
        } finally {
            setLoadingDevices(false);
        }
    };

    // Check current device mapping
    const handleCheckCurrentDevice = async () => {
        try {
            const hostname = window.location.hostname;
            const response = await mappingDeviceService.getMappingByStaffDeviceAsync(hostname);

            if (response) {
                setCurrentDeviceMapping(response);
                Swal.fire({
                    icon: 'info',
                    title: 'Current Device Mapping',
                    html: `
                        <div style="text-align: left;">
                            <p><strong>ID:</strong> ${response.id}</p>
                            <p><strong>Staff Device:</strong> ${response.staffDeviceName}</p>
                            <p><strong>Patron Device:</strong> ${response.patronDeviceName}</p>
                            <p><strong>Location:</strong> ${response.location}</p>
                            <p><strong>Status:</strong> ${response.isActive ? '<span style="color: green;">Active</span>' : '<span style="color: red;">Inactive</span>'}</p>
                            <p><strong>Last Verified:</strong> ${new Date(response.lastVerified).toLocaleString()}</p>
                        </div>
                    `,
                    confirmButtonText: 'OK'
                });
            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'No Mapping Found',
                    text: `No mapping found for current device: ${hostname}`,
                    confirmButtonText: 'OK'
                });
            }
        } catch (err) {
            console.error('Error checking current device:', err);
            showSnackbar('Failed to check current device mapping', 'error');
        }
    };

    // Handle create mapping
    const handleCreateMapping = async () => {
        if (!createForm.StaffDeviceName || !createForm.PatronDeviceName || !createForm.OutletId) {
            showSnackbar('Please fill in all required fields', 'error');
            return;
        }

        try {
            await mappingDeviceService.createMappingAsync(createForm);
            showSnackbar('Mapping created successfully', 'success');
            setCreateDialogOpen(false);
            setCreateForm({
                StaffDeviceName: '',
                PatronDeviceName: '',
                Notes: '',
                OutletId: 0,
            });
            loadMappings();
        } catch (err) {
            console.error('Error creating mapping:', err);
            showSnackbar('Failed to create mapping', 'error');
        }
    };

    // Handle open update dialog
    const handleOpenUpdateDialog = (mapping: MappingDataResponse) => {
        setUpdateForm({
            Id: mapping.id,
            NewStaffDeviceName: mapping.staffDeviceName,
            NewPatronDeviceName: mapping.patronDeviceName,
            Location: mapping.location,
            Notes: mapping.notes || '',
            OutletId: mapping.outletId || 0,
            OutletName: mapping.outletName || ''
        });
        setUpdateDialogOpen(true);
        loadDevices();
    };

    // Handle update mapping
    const handleUpdateMapping = async () => {
        if (!updateForm.NewStaffDeviceName || !updateForm.NewPatronDeviceName || !updateForm.OutletId) {
            showSnackbar('Please fill in all required fields', 'error');
            return;
        }

        try {
            await mappingDeviceService.updateMappingAsync(updateForm);
            showSnackbar('Mapping updated successfully', 'success');
            setUpdateDialogOpen(false);
            loadMappings();
        } catch (err) {
            console.error('Error updating mapping:', err);
            showSnackbar('Failed to update mapping', 'error');
        }
    };

    // Handle delete mapping
    const handleDeleteMapping = async (mapping: MappingDataResponse) => {
        const result = await Swal.fire({
            icon: 'warning',
            title: 'Confirm Delete',
            html: `
                <p>Are you sure you want to delete this mapping?</p>
                <p><strong>Staff Device:</strong> ${mapping.staffDeviceName}</p>
                <p><strong>Patron Device:</strong> ${mapping.patronDeviceName}</p>
            `,
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it',
            confirmButtonColor: '#d33',
            cancelButtonText: 'Cancel'
        });

        if (result.isConfirmed) {
            try {
                await mappingDeviceService.deleteMappingAsync(mapping.id);
                showSnackbar('Mapping deleted successfully', 'success');
                loadMappings();
            } catch (error: unknown) {
                logError('DeviceMappingSettingsPage.handleDeleteMapping', error);
                const errorMessage = extractErrorMessage(error, 'Failed to delete mapping.');
                showSnackbar(errorMessage, 'error');
            }
        }
    };

    const handleCheckHostname = async () => {
        try {
            const data = await staffDeviceService.getCurrentHostNameAsync();
            setHostnameData(data);
            setHostnameDialogOpen(true);
        } catch (error: unknown) {
            logError('DeviceMappingSettingsPage.handleCheckHostname', error);
            const errorMessage = extractErrorMessage(error, 'Error getting hostname information');
            showSnackbar(errorMessage, "error");
        }
    };

    const handleCopyToClipboard = async (text: string, label: string) => {
        try {
            await navigator.clipboard.writeText(text);
            showSnackbar(`${label} copied to clipboard!`, "success");
        } catch (error) {
            console.error("Failed to copy to clipboard:", error);
            showSnackbar(`Failed to copy ${label}`, "error");
        }
    };

    // Handle pagination
    const handlePageChange = (_event: unknown, newPage: number) => {
        setPage(newPage);
    };

    const handleRowsPerPageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    // Calculate paginated data
    const paginatedMappings = filteredMappings.slice(
        page * rowsPerPage,
        page * rowsPerPage + rowsPerPage
    );

    return (
        <AdminLayout>
            <Box sx={{ p: 3 }}>
                {/* Header */}
                <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ mb: 3 }}>
                    <Stack direction="row" spacing={2}>
                        <Button
                            variant="outlined"
                            startIcon={<DevicesIcon />}
                            onClick={handleCheckCurrentDevice}
                        >
                            Check Current Device
                        </Button>
                        <Button
                            variant="outlined"
                            startIcon={<ComputerIcon />}
                            onClick={handleCheckHostname}
                        >
                            Check Hostname
                        </Button>
                        <Button
                            variant="contained"
                            startIcon={<AddIcon />}
                            onClick={() => {
                                setCreateDialogOpen(true);
                                loadDevices();
                            }}
                        >
                            Create Mapping
                        </Button>
                    </Stack>
                </Box>

                {/* Current Device Info */}
                {currentDeviceMapping && (
                    <Alert severity="info" sx={{ mb: 2 }} onClose={() => setCurrentDeviceMapping(null)}>
                        <strong>Current Device:</strong> {currentDeviceMapping.staffDeviceName} → {currentDeviceMapping.patronDeviceName}
                    </Alert>
                )}

                {/* Mappings Table */}
                <Card>
                    <CardContent>
                        <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
                            <Typography variant="h6">
                                Device Mappings ({filteredMappings.length})
                            </Typography>
                            <IconButton onClick={loadMappings} disabled={loading} color="primary">
                                <RefreshIcon />
                            </IconButton>
                        </Box>

                        <Box display="flex" alignItems="center" sx={{ mb: 2, gap: 2, flexWrap: 'wrap' }}>
                            {/* Search Bar */}
                            <TextField
                                placeholder="Search by Device Name, Location, Property or Notes..."
                                value={searchQuery}
                                onChange={(e) => setSearchQuery(e.target.value)}
                                size='small'
                                InputProps={{
                                    startAdornment: (
                                        <IconButton size="small">
                                            <Search />
                                        </IconButton>
                                    ),
                                }}
                                sx={{ width: '450px' }}
                            />

                            {/* <Autocomplete
                                options={outlets}
                                size="small"
                                getOptionLabel={(option: OutletResponse) => option.name}
                                value={searchQuery ? outlets.find(o => o.name.toLowerCase() === searchQuery.toLowerCase()) || null : null}
                                onChange={(_, newValue) => setSearchQuery(newValue ? newValue.name : '')}
                                isOptionEqualToValue={(option, value) => option.id === value.id}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Outlet"
                                        placeholder="Select outlet"
                                    />
                                )}
                                sx={{ width: '250px' }}
                            /> */}
                        </Box>


                        {loading ? (
                            <Box display="flex" justifyContent="center" py={4}>
                                <CircularProgress />
                            </Box>
                        ) : (
                            <>
                                <TableContainer component={Paper} sx={{ maxHeight: 600 }}>
                                    <Table>
                                        <TableHead>
                                            <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                                                <TableCell align='center'
                                                    sx={{
                                                        position: 'sticky',
                                                        left: 0,
                                                        backgroundColor: muiTheme.palette.background.paper,
                                                        zIndex: 3,
                                                        boxShadow: '2px 0 5px rgba(0,0,0,0.1)',
                                                        bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 }
                                                    }}>
                                                    Actions
                                                </TableCell>
                                                <TableCell>ID</TableCell>
                                                <TableCell sx={{ minWidth: 120 }}>Staff Device</TableCell>
                                                {/* <TableCell sx={{ minWidth: 120 }}>Staff IP</TableCell> */}
                                                {/* <TableCell sx={{ minWidth: 120 }}>Staff Online</TableCell> */}
                                                <TableCell sx={{ minWidth: 120 }}>Patron Device</TableCell>
                                                {/* <TableCell sx={{ minWidth: 120 }}>Patron IP</TableCell> */}
                                                {/* <TableCell sx={{ minWidth: 120 }}>Patron Online</TableCell> */}
                                                <TableCell sx={{ minWidth: 120 }}>Location</TableCell>
                                                <TableCell sx={{ minWidth: 120 }}>Property</TableCell>
                                                <TableCell sx={{ minWidth: 120 }}>Notes</TableCell>
                                                <TableCell>Status</TableCell>
                                                <TableCell sx={{ minWidth: 150 }}>Last Verified</TableCell>
                                                {/* <TableCell sx={{ minWidth: 150 }}>Created At</TableCell> */}

                                            </TableRow>
                                        </TableHead>
                                        <TableBody>
                                            {paginatedMappings.map((mapping) => (
                                                <TableRow key={mapping.id} hover>
                                                    <TableCell
                                                        sx={{
                                                            position: 'sticky',
                                                            left: 0,
                                                            backgroundColor: muiTheme.palette.background.paper,
                                                            zIndex: 1,
                                                            boxShadow: '2px 0 5px rgba(0,0,0,0.1)'
                                                        }}>
                                                        <Stack direction="row" spacing={1} justifyContent="center">
                                                            <Tooltip title="Edit">
                                                                <IconButton
                                                                    size="small"
                                                                    color="primary"
                                                                    onClick={() => handleOpenUpdateDialog(mapping)}
                                                                >
                                                                    <EditIcon />
                                                                </IconButton>
                                                            </Tooltip>
                                                            <Tooltip title="Delete">
                                                                <IconButton
                                                                    size="small"
                                                                    color="error"
                                                                    onClick={() => handleDeleteMapping(mapping)}
                                                                >
                                                                    <DeleteIcon />
                                                                </IconButton>
                                                            </Tooltip>
                                                        </Stack>
                                                    </TableCell>
                                                    <TableCell>{mapping.id}</TableCell>
                                                    <TableCell>
                                                        <Typography variant="body2" sx={{ fontWeight: 600 }}>
                                                            {mapping.staffDeviceName}
                                                        </Typography>

                                                        <Typography variant="caption" color="textSecondary" sx={{ display: 'flex', alignItems: 'center', gap: 0.5, pt: 0.5 }}>
                                                            Status:
                                                            <Chip
                                                                label={mapping.staffIsOnline ? 'Online' : 'Offline'}
                                                                color={mapping.staffIsOnline ? 'success' : 'error'}
                                                                size="small"
                                                            />
                                                        </Typography>

                                                    </TableCell>
                                                    {/* <TableCell>{mapping.staffIp || '-'}</TableCell> */}
                                                    {/* <TableCell>
                                                        <Chip
                                                            label={mapping.staffDeviceIsOnline ? 'Online' : 'Offline'}
                                                            color={mapping.staffDeviceIsOnline ? 'success' : 'error'}
                                                            size="small"
                                                        />
                                                    </TableCell> */}
                                                    <TableCell>
                                                        <Typography variant="body2" sx={{ fontWeight: 600 }}>
                                                            {mapping.patronDeviceName}
                                                        </Typography>
                                                        <Typography variant="caption" color="textSecondary" sx={{ display: 'flex', alignItems: 'center', gap: 0.5, pt: 0.5 }}>
                                                            Status:
                                                            <Chip
                                                                label={mapping.patronIsOnline ? 'Online' : 'Offline'}
                                                                color={mapping.patronIsOnline ? 'success' : 'error'}
                                                                size="small"
                                                            />
                                                        </Typography>
                                                    </TableCell>
                                                    {/* <TableCell>{mapping.patronIp || '-'}</TableCell> */}
                                                    {/* <TableCell>
                                                        <Chip
                                                            label={mapping.patronIsOnline ? 'Online' : 'Offline'}
                                                            color={mapping.patronIsOnline ? 'success' : 'error'}
                                                            size="small"
                                                        />
                                                    </TableCell> */}
                                                    <TableCell>{mapping.location || mapping.outletName || '-'}</TableCell>
                                                    <TableCell>{mapping.propertyName}</TableCell>
                                                    <TableCell>{mapping.notes || '-'}</TableCell>
                                                    <TableCell>
                                                        <Chip
                                                            label={mapping.isActive ? 'Active' : 'Inactive'}
                                                            color={mapping.isActive ? 'success' : 'default'}
                                                            size="small"
                                                        />
                                                    </TableCell>
                                                    <TableCell>{FormatUtcTime.formatDateTime(mapping.lastVerified)}</TableCell>

                                                    {/* <TableCell>{FormatUtcTime.formatDateTime(mapping.createdAt)}</TableCell> */}

                                                </TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </TableContainer>
                                <TablePagination
                                    component="div"
                                    count={filteredMappings.length}
                                    page={page}
                                    onPageChange={handlePageChange}
                                    rowsPerPage={rowsPerPage}
                                    onRowsPerPageChange={handleRowsPerPageChange}
                                    rowsPerPageOptions={[5, 10, 25, 50, 100]}
                                />
                            </>
                        )}
                    </CardContent>
                </Card>

                {/* Create Mapping Dialog */}
                <Dialog
                    open={createDialogOpen}
                    onClose={() => setCreateDialogOpen(false)}
                    maxWidth="sm"
                    fullWidth
                >
                    <DialogTitle>Create New Mapping</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 2 }}>
                            <Autocomplete
                                options={staffDevices}
                                getOptionLabel={(option) => option.staffDeviceName}
                                value={staffDevices.find(d => d.staffDeviceName === createForm.StaffDeviceName) || null}
                                onChange={(_, newValue) => {
                                    setCreateForm({ ...createForm, StaffDeviceName: newValue?.staffDeviceName || '' });
                                }}
                                loading={loadingDevices}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Staff Device Name *"
                                        placeholder="Select staff device"
                                    />
                                )}
                                fullWidth
                            />
                            <Autocomplete
                                options={patronDevices}
                                getOptionLabel={(option) => option.patronDeviceName}
                                value={patronDevices.find(d => d.patronDeviceName === createForm.PatronDeviceName) || null}
                                onChange={(_, newValue) => {
                                    setCreateForm({ ...createForm, PatronDeviceName: newValue?.patronDeviceName || '' });
                                }}
                                loading={loadingDevices}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Patron Device Name *"
                                        placeholder="Select patron device"
                                    />
                                )}
                                fullWidth
                            />
                            <Autocomplete
                                options={outlets}
                                getOptionLabel={(option: OutletResponse) => option.name}
                                value={outlets.find(o => o.id === createForm.OutletId) || null}
                                onChange={(_, newValue) => setCreateForm({ ...createForm, OutletId: newValue?.id || 0 })}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Outlet *"
                                        placeholder="Select outlet"
                                    />
                                )}
                                fullWidth
                            />

                            <TextField
                                label="Notes"
                                value={createForm.Notes}
                                onChange={(e) => setCreateForm({ ...createForm, Notes: e.target.value })}
                                fullWidth
                                multiline
                                rows={3}
                                placeholder="Additional notes..."
                            />
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setCreateDialogOpen(false)}>Cancel</Button>
                        <Button
                            variant="contained"
                            onClick={handleCreateMapping}
                            startIcon={<AddIcon />}
                        >
                            Create
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* Update Mapping Dialog */}
                <Dialog
                    open={updateDialogOpen}
                    onClose={() => setUpdateDialogOpen(false)}
                    maxWidth="sm"
                    fullWidth
                >
                    <DialogTitle>Update Mapping</DialogTitle>
                    <DialogContent>
                        <Stack spacing={2} sx={{ mt: 2 }}>
                            <TextField
                                label="Mapping ID"
                                value={updateForm.Id}
                                disabled
                                fullWidth
                            />
                            <Autocomplete
                                options={staffDevices}
                                getOptionLabel={(option) => option.staffDeviceName}
                                value={staffDevices.find(d => d.staffDeviceName === updateForm.NewStaffDeviceName) || null}
                                onChange={(_, newValue) => {
                                    setUpdateForm({ ...updateForm, NewStaffDeviceName: newValue?.staffDeviceName || '' });
                                }}
                                loading={loadingDevices}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Staff Device Name *"
                                        placeholder="Select staff device"
                                    />
                                )}
                                fullWidth
                            />
                            <Autocomplete
                                options={patronDevices}
                                getOptionLabel={(option) => option.patronDeviceName}
                                value={patronDevices.find(d => d.patronDeviceName === updateForm.NewPatronDeviceName) || null}
                                onChange={(_, newValue) => {
                                    setUpdateForm({ ...updateForm, NewPatronDeviceName: newValue?.patronDeviceName || '' });
                                }}
                                loading={loadingDevices}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Patron Device Name *"
                                        placeholder="Select patron device"
                                    />
                                )}
                                fullWidth
                            />
                            <Autocomplete
                                options={outlets}
                                getOptionLabel={(option: OutletResponse) => option.name}
                                value={outlets.find(o => o.id === updateForm.OutletId) || null}
                                onChange={(_, newValue) => setUpdateForm({ ...updateForm, OutletId: newValue?.id || 0, OutletName: newValue?.name || '' })}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Outlet"
                                        placeholder="Select outlet"
                                    />
                                )}
                                fullWidth
                            />
                            <TextField
                                label="Notes"
                                value={updateForm.Notes}
                                onChange={(e) => setUpdateForm({ ...updateForm, Notes: e.target.value })}
                                fullWidth
                                multiline
                                rows={3}
                                placeholder="Additional notes..."
                            />
                        </Stack>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setUpdateDialogOpen(false)}>Cancel</Button>
                        <Button
                            variant="contained"
                            onClick={handleUpdateMapping}
                            startIcon={<CheckCircleIcon />}
                        >
                            Update
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* Hostname Information Dialog */}
                <Dialog
                    open={hostnameDialogOpen}
                    onClose={() => setHostnameDialogOpen(false)}
                    maxWidth="sm"
                    fullWidth
                >
                    <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <ComputerIcon />
                        Current Computer Information
                    </DialogTitle>
                    <DialogContent>
                        {hostnameData && (
                            <List>
                                <ListItem sx={{ px: 0 }}>
                                    <ListItemText
                                        primary="Computer Name"
                                        secondary={hostnameData.computerName}
                                        primaryTypographyProps={{ fontWeight: 600 }}
                                        secondaryTypographyProps={{
                                            fontSize: '1.1rem',
                                            color: 'primary.main',
                                            fontFamily: 'monospace'
                                        }}
                                    />
                                    <ListItemSecondaryAction>
                                        <Tooltip title="Copy Computer Name">
                                            <IconButton
                                                edge="end"
                                                onClick={() => handleCopyToClipboard(hostnameData.computerName, "Computer Name")}
                                                size="small"
                                            >
                                                <ContentCopyIcon />
                                            </IconButton>
                                        </Tooltip>
                                    </ListItemSecondaryAction>
                                </ListItem>

                                <ListItem sx={{ px: 0 }}>
                                    <ListItemText
                                        primary="IP Address"
                                        secondary={hostnameData.ip}
                                        primaryTypographyProps={{ fontWeight: 600 }}
                                        secondaryTypographyProps={{
                                            fontSize: '1.1rem',
                                            color: 'secondary.main',
                                            fontFamily: 'monospace'
                                        }}
                                    />
                                    <ListItemSecondaryAction>
                                        <Tooltip title="Copy IP Address">
                                            <IconButton
                                                edge="end"
                                                onClick={() => handleCopyToClipboard(hostnameData.ip, "IP Address")}
                                                size="small"
                                            >
                                                <ContentCopyIcon />
                                            </IconButton>
                                        </Tooltip>
                                    </ListItemSecondaryAction>
                                </ListItem>
                            </List>
                        )}
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setHostnameDialogOpen(false)} variant="contained">
                            Close
                        </Button>
                    </DialogActions>
                </Dialog>
            </Box>
        </AdminLayout>
    );
};

export default DeviceMappingSettingsPage;

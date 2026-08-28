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
    TextField,
    Button,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    LinearProgress,
    Alert,
    Skeleton,
    Pagination,
    Chip,
    IconButton,
    Tooltip,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Switch,
    FormControlLabel,
    InputAdornment,
    Checkbox,
    ListItemText,
    OutlinedInput,
} from "@mui/material";
import {
    Search as SearchIcon,
    Refresh as RefreshIcon,
    Add as AddIcon,
    Edit as EditIcon,
    Delete as DeleteIcon,
    Business as BusinessIcon,
    Circle as CircleIcon,
} from "@mui/icons-material";
import { useState, useEffect, useMemo } from "react";
import AdminLayout from "../../components/layout/AdminLayout";
import { propertyService } from "../../services/propertyService";
import { outletService } from "../../services/outletService";
import { useDebounce } from "../../hooks/useDebounce";
import type { PropertyResponse, CreatePropertyRequest, UpdatePropertyRequest } from "../../types/propertyType";
import type { OutletResponse } from "../../types/outletType";
import { useSetPageTitle } from "../../hooks/useSetPageTitle";
import { extractErrorMessage, logError } from "../../utils/errorHandler";
import { getApiBase } from "../../utils/envConfig";
import { useSnackbar } from "../../contexts/SnackbarContext";
import { PAGE_TITLES } from "../../constants/pageTitles";
import { useTheme } from '@mui/material/styles';
const COLOR_OPTIONS = [
    { value: "#1976d2", label: "Blue" },
    { value: "#388e3c", label: "Green" },
    { value: "#d32f2f", label: "Red" },
    { value: "#f57c00", label: "Orange" },
    { value: "#7b1fa2", label: "Purple" },
    { value: "#0288d1", label: "Light Blue" },
    { value: "#455a64", label: "Gray" },
    { value: "#c2185b", label: "Pink" },
    { value: "#00796b", label: "Teal" },
    { value: "#e65100", label: "Deep Orange" },
];

const ITEMS_PER_PAGE_OPTIONS = [5, 10, 20, 50];

export default function AdminPropertyPage() {
    useSetPageTitle(PAGE_TITLES.PROPERTY_MANAGEMENT);

    const { showSnackbar } = useSnackbar();

    const muiTheme = useTheme();
    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [allOutlets, setAllOutlets] = useState<OutletResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState("");
    const debouncedSearch = useDebounce(searchTerm, 300);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(10);

    // Dialog states
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");
    const [dialogLoading, setDialogLoading] = useState(false);
    const [editingProperty, setEditingProperty] = useState<PropertyResponse | null>(null);
    const [formData, setFormData] = useState<CreatePropertyRequest>({
        Name: "",
        Description: "",
        Color: "#1976d2",
        OutletIds: [],
    });
    const [formIsActive, setFormIsActive] = useState(true);

    // Delete dialog
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [deletingProperty, setDeletingProperty] = useState<PropertyResponse | null>(null);
    const [deleteLoading, setDeleteLoading] = useState(false);

    // Load list
    const loadProperties = async () => {
        try {
            setLoading(true);
            setError(null);
            const [data, outletData] = await Promise.all([
                propertyService.getListAsync(),
                outletService.getListAsync(),
            ]);
            setProperties(data);
            setAllOutlets(outletData);
        } catch (err: unknown) {
            logError("AdminPropertyPage.loadProperties", err);
            setError(extractErrorMessage(err, "Failed to load properties"));
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadProperties();
    }, []);

    // Filter + paginate (client-side)
    const filtered = useMemo(() => {
        const q = debouncedSearch.toLowerCase();
        return properties.filter(
            (p) =>
                p.name.toLowerCase().includes(q) ||
                p.description.toLowerCase().includes(q),
        );
    }, [properties, debouncedSearch]);

    const totalPages = Math.ceil(filtered.length / itemsPerPage);
    const paginated = useMemo(() => {
        const start = (currentPage - 1) * itemsPerPage;
        return filtered.slice(start, start + itemsPerPage);
    }, [filtered, currentPage, itemsPerPage]);

    useEffect(() => {
        setCurrentPage(1);
    }, [debouncedSearch, itemsPerPage]);

    const lockedOutletIds = useMemo(() => {
        if (dialogMode !== "edit" || !editingProperty) return [] as number[];

        return allOutlets
            .filter((o) => o.properties?.some((p) => p.id === editingProperty.id && p.isPrimaryOutlet))
            .map((o) => o.id);
    }, [dialogMode, editingProperty, allOutlets]);

    // ── Dialog handlers ──────────────────────────────────────────────────────

    const handleOpenCreate = () => {
        setDialogMode("create");
        setEditingProperty(null);
        setFormData({ Name: "", Description: "", Color: "#1976d2", OutletIds: [] });
        setFormIsActive(true);
        setDialogOpen(true);
    };

    const handleOpenEdit = (property: PropertyResponse) => {
        setDialogMode("edit");
        setEditingProperty(property);
        setFormData({
            Name: property.name,
            Description: property.description,
            Color: property.color,
            OutletIds: property.outlets?.map((o) => o.id) ?? [],
        });
        setFormIsActive(property.isActive);
        setDialogOpen(true);
    };

    const handleCloseDialog = () => {
        if (!dialogLoading) setDialogOpen(false);
    };

    const handleSubmit = async () => {
        if (!formData.Name.trim()) return;
        try {
            setDialogLoading(true);
            if (dialogMode === "create") {
                await propertyService.createAsync(formData);
                showSnackbar("Property created successfully.", "success");
            } else if (editingProperty) {
                const req: UpdatePropertyRequest = {
                    Id: editingProperty.id,
                    Name: formData.Name,
                    Description: formData.Description,
                    Color: formData.Color,
                    IsActive: formIsActive,
                    OutletIds: formData.OutletIds ?? [],
                };
                await propertyService.updateAsync(req);
                showSnackbar("Property updated successfully.", "success");
            }
            setDialogOpen(false);
            await loadProperties();
        } catch (err: unknown) {
            logError("AdminPropertyPage.handleSubmit", err);
            showSnackbar(extractErrorMessage(err, "Failed to save property"), "error");
        } finally {
            setDialogLoading(false);
        }
    };

    // ── Delete handlers ──────────────────────────────────────────────────────

    const handleOpenDelete = (property: PropertyResponse) => {
        setDeletingProperty(property);
        setDeleteDialogOpen(true);
    };

    const handleCloseDelete = () => {
        if (!deleteLoading) setDeleteDialogOpen(false);
    };

    const handleDelete = async () => {
        if (!deletingProperty) return;
        try {
            setDeleteLoading(true);
            await propertyService.deleteAsync(deletingProperty.id);
            showSnackbar("Property deleted successfully.", "success");
            setDeleteDialogOpen(false);
            await loadProperties();
        } catch (err: unknown) {
            logError("AdminPropertyPage.handleDelete", err);
            showSnackbar(extractErrorMessage(err, "Failed to delete property"), "error");
        } finally {
            setDeleteLoading(false);
        }
    };

    return (
        <AdminLayout>
            <Box sx={{ p: { xs: 2, md: 3 } }}>
                {/* Header */}
                <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 3, flexWrap: "wrap", gap: 2 }}>
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                        <BusinessIcon color="primary" sx={{ fontSize: 28 }} />
                        <Box>
                            {/* <Typography variant="h5" fontWeight={700}>
                                Property Management
                            </Typography> */}
                            <Typography variant="body2" color="text.secondary">
                                Management of properties this application is running on. Each property can have multiple devices mapped to it.
                            </Typography>
                        </Box>
                    </Box>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={handleOpenCreate}
                    >
                        Add Property
                    </Button>
                </Box>

                {/* Filters */}
                <Card sx={{ mb: 2 }}>
                    <CardContent sx={{ pb: "12px !important" }}>
                        <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap", alignItems: "center" }}>
                            <TextField
                                size="small"
                                placeholder="Search by name or description..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                sx={{ width: { xs: "100%", sm: 350 } }}
                                InputProps={{
                                    startAdornment: (
                                        <InputAdornment position="start">
                                            <SearchIcon fontSize="small" />
                                        </InputAdornment>
                                    ),
                                }}
                            />
                            <FormControl size="small" sx={{ minWidth: 100 }}>
                                <InputLabel>Per page</InputLabel>
                                <Select
                                    label="Per page"
                                    value={itemsPerPage}
                                    onChange={(e) => setItemsPerPage(Number(e.target.value))}
                                >
                                    {ITEMS_PER_PAGE_OPTIONS.map((n) => (
                                        <MenuItem key={n} value={n}>{n}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <Tooltip title="Refresh">
                                <IconButton onClick={loadProperties} disabled={loading}>
                                    <RefreshIcon />
                                </IconButton>
                            </Tooltip>
                            <Typography variant="body2" color="text.secondary" sx={{ ml: "auto" }}>
                                {filtered.length} result{filtered.length !== 1 ? "s" : ""}
                            </Typography>
                        </Box>
                    </CardContent>
                </Card>

                {/* Table */}
                <Card>
                    {loading && <LinearProgress />}
                    <TableContainer component={Paper} elevation={0}>
                        <Table>
                            <TableHead>
                                <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                                    <TableCell width={60}>#</TableCell>
                                    <TableCell>NAME</TableCell>
                                    <TableCell>CODE</TableCell>
                                    <TableCell>DESCRIPTION</TableCell>
                                    <TableCell>OUTLETS</TableCell>
                                    <TableCell width={90}>COLOR</TableCell>
                                    <TableCell width={90}>STATUS</TableCell>
                                    <TableCell width={100} align="center">ACTIONS</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {loading ? (
                                    Array.from({ length: 5 }).map((_, i) => (
                                        <TableRow key={i}>
                                            {Array.from({ length: 7 }).map((__, j) => (
                                                <TableCell key={j}><Skeleton /></TableCell>
                                            ))}
                                        </TableRow>
                                    ))
                                ) : error ? (
                                    <TableRow>
                                        <TableCell colSpan={8}>
                                            <Alert severity="error">{error}</Alert>
                                        </TableCell>
                                    </TableRow>
                                ) : paginated.length === 0 ? (
                                    <TableRow>
                                        <TableCell colSpan={8} align="center">
                                            <Typography color="text.secondary" py={3}>
                                                No properties found.
                                            </Typography>
                                        </TableCell>
                                    </TableRow>
                                ) : (
                                    paginated.map((property, idx) => (
                                        <TableRow key={property.id} hover>
                                            <TableCell>{(currentPage - 1) * itemsPerPage + idx + 1}</TableCell>
                                            <TableCell>
                                                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                                    <CircleIcon sx={{ color: property.color, fontSize: 14 }} />
                                                    <Typography fontWeight={600}>{property.name}</Typography>
                                                    {/* {property.isPrimaryOutlet && (
                                                        <Chip
                                                            variant="outlined"
                                                            size="small"
                                                            color="warning"
                                                            label="Main Outlet"
                                                            sx={{ fontWeight: 700 }}
                                                        />
                                                    )} */}
                                                </Box>
                                            </TableCell>
                                            <TableCell>
                                                <Chip variant="outlined"
                                                    size="small"
                                                    label={property.code || "—"}
                                                    sx={{
                                                        borderColor: property.color,
                                                        color: property.color,
                                                        fontWeight: 700,
                                                        fontSize: "0.8rem"
                                                    }} />
                                            </TableCell>
                                            <TableCell>
                                                <Typography variant="body2" color="text.secondary" noWrap sx={{ maxWidth: 250 }}>
                                                    {property.description || "—"}
                                                </Typography>
                                            </TableCell>
                                            <TableCell>
                                                <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                                                    {property.outlets && property.outlets.length > 0
                                                        ? property.outlets.map((o) => (
                                                            <Tooltip key={o.id} title={o.name}>
                                                                <Box component="img"
                                                                    src={getApiBase() + (o.iconImageUrl)}
                                                                    alt={o.name}
                                                                    sx={{ width: 50, height: 50, objectFit: "contain", borderRadius: 1, ml: 1.5 }}
                                                                    onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }}>
                                                                </Box>
                                                            </Tooltip>
                                                        ))
                                                        : <Typography variant="body2" color="text.secondary">—</Typography>
                                                    }
                                                </Box>
                                            </TableCell>
                                            <TableCell>
                                                <Chip
                                                    size="small"
                                                    label={property.color}
                                                    sx={{ bgcolor: property.color, color: "#fff", fontWeight: 700, fontSize: "0.68rem" }}
                                                />
                                            </TableCell>
                                            <TableCell>
                                                <Chip
                                                    size="small"
                                                    label={property.isActive ? "Active" : "Inactive"}
                                                    color={property.isActive ? "success" : "error"}
                                                />
                                            </TableCell>
                                            <TableCell align="center">
                                                <Tooltip title="Edit">
                                                    <IconButton size="small" onClick={() => handleOpenEdit(property)}>
                                                        <EditIcon fontSize="small" />
                                                    </IconButton>
                                                </Tooltip>
                                                <Tooltip title="Delete">
                                                    <IconButton size="small" color="error" onClick={() => handleOpenDelete(property)}>
                                                        <DeleteIcon fontSize="small" />
                                                    </IconButton>
                                                </Tooltip>
                                            </TableCell>
                                        </TableRow>
                                    ))
                                )}
                            </TableBody>
                        </Table>
                    </TableContainer>

                    {/* Pagination */}
                    {!loading && totalPages > 1 && (
                        <Box sx={{ display: "flex", justifyContent: "center", p: 2 }}>
                            <Pagination
                                count={totalPages}
                                page={currentPage}
                                onChange={(_, page) => setCurrentPage(page)}
                                color="primary"
                                size="small"
                            />
                        </Box>
                    )}
                </Card>

                {/* Create / Edit Dialog */}
                <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
                    <DialogTitle>
                        {dialogMode === "create" ? "Create Property" : "Edit Property"}
                    </DialogTitle>
                    <DialogContent dividers>
                        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, pt: 1 }}>
                            <TextField
                                label="Name *"
                                value={formData.Name}
                                onChange={(e) => setFormData({ ...formData, Name: e.target.value })}
                                fullWidth
                                size="small"
                                autoFocus
                            />
                            <TextField
                                label="Description"
                                value={formData.Description}
                                onChange={(e) => setFormData({ ...formData, Description: e.target.value })}
                                fullWidth
                                size="small"
                                multiline
                                rows={2}
                            />
                            <FormControl size="small" fullWidth>
                                <InputLabel>Color</InputLabel>
                                <Select
                                    label="Color"
                                    value={formData.Color}
                                    onChange={(e) => setFormData({ ...formData, Color: e.target.value })}
                                    renderValue={(val) => (
                                        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                            <CircleIcon sx={{ color: val, fontSize: 16 }} />
                                            {COLOR_OPTIONS.find((c) => c.value === val)?.label ?? val}
                                        </Box>
                                    )}
                                >
                                    {COLOR_OPTIONS.map((c) => (
                                        <MenuItem key={c.value} value={c.value}>
                                            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                                <CircleIcon sx={{ color: c.value, fontSize: 16 }} />
                                                {c.label}
                                            </Box>
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>

                            <FormControl size="small" fullWidth>
                                <InputLabel>Outlets</InputLabel>
                                <Select
                                    multiple
                                    label="Outlets"
                                    value={formData.OutletIds ?? []}
                                    onChange={(e) => {
                                        const val = e.target.value;
                                        setFormData({ ...formData, OutletIds: typeof val === "string" ? val.split(",").map(Number) : val as number[] });
                                    }}
                                    input={<OutlinedInput label="Outlets" />}
                                    renderValue={(selected) =>
                                        (selected as number[])
                                            .map((id) => allOutlets.find((o) => o.id === id)?.name ?? `#${id}`)
                                            .join(", ")
                                    }
                                >
                                    {allOutlets.map((o) => (
                                        <MenuItem
                                            key={o.id}
                                            value={o.id}
                                            disabled={lockedOutletIds.includes(o.id)}
                                        >
                                            <Checkbox
                                                checked={(formData.OutletIds ?? []).includes(o.id)}
                                                disabled={lockedOutletIds.includes(o.id)}
                                            />
                                            <ListItemText
                                                primary={o.name}
                                                secondary={lockedOutletIds.includes(o.id) ? "Main outlet" : undefined}
                                            />
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            {lockedOutletIds.length > 0 && (
                                <Alert severity="info">
                                    Main outlet is locked and cannot be changed. You can still select other outlets.
                                </Alert>
                            )}

                            {dialogMode === "edit" && (
                                <FormControlLabel
                                    control={
                                        <Switch
                                            checked={formIsActive}
                                            onChange={(e) => setFormIsActive(e.target.checked)}
                                            color="success"
                                        />
                                    }
                                    label="Active"
                                />
                            )}

                            {/* Preview */}
                            <Box sx={{ p: 2, bgcolor: "background.default", borderRadius: 1 }}>
                                <Typography variant="body2" color="text.secondary" gutterBottom>
                                    Preview:
                                </Typography>
                                <Chip
                                    icon={<BusinessIcon />}
                                    label={formData.Name || "Property Name"}
                                    sx={{ bgcolor: formData.Color, color: "#fff", fontWeight: 700 }}
                                />
                            </Box>
                        </Box>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCloseDialog} disabled={dialogLoading}>
                            Cancel
                        </Button>
                        <Button
                            variant="contained"
                            onClick={handleSubmit}
                            disabled={dialogLoading || !formData.Name.trim()}
                        >
                            {dialogLoading ? "Saving..." : dialogMode === "create" ? "Create" : "Update"}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* Delete Confirmation Dialog */}
                <Dialog open={deleteDialogOpen} onClose={handleCloseDelete}>
                    <DialogTitle>Confirm Delete</DialogTitle>
                    <DialogContent>
                        <Typography>
                            Are you sure you want to delete property{" "}
                            <Chip
                                label={deletingProperty?.name}
                                size="small"
                                sx={{ bgcolor: deletingProperty?.color, color: "#fff", fontWeight: 700 }}
                            />
                            ?
                        </Typography>
                        <Alert severity="warning" sx={{ mt: 2 }}>
                            This action cannot be undone. All devices mapped to this property will be unmapped.
                        </Alert>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCloseDelete} disabled={deleteLoading}>
                            Cancel
                        </Button>
                        <Button onClick={handleDelete} color="error" variant="contained" disabled={deleteLoading}>
                            {deleteLoading ? "Deleting..." : "Delete"}
                        </Button>
                    </DialogActions>
                </Dialog>
            </Box>
        </AdminLayout >
    );
}

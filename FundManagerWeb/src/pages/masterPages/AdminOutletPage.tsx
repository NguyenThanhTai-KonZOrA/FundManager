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
    ListItemText,
    Checkbox,
    OutlinedInput,
} from "@mui/material";
import {
    Search as SearchIcon,
    Refresh as RefreshIcon,
    Add as AddIcon,
    Edit as EditIcon,
    Delete as DeleteIcon,
    StoreMallDirectory as OutletIcon,
} from "@mui/icons-material";
import { useState, useEffect, useMemo } from "react";
import AdminLayout from "../../components/layout/AdminLayout";
import { outletService } from "../../services/outletService";
import { propertyService } from "../../services/propertyService";
import { applicationImageService } from "../../services/applicationImageService";
import { useDebounce } from "../../hooks/useDebounce";
import type { OutletResponse, CreateOutletRequest, UpdateOutletRequest } from "../../types/outletType";
import type { PropertyResponse } from "../../types/propertyType";
import type { ApplicationImageResponse } from "../../types/applicationImageType";
import { ImageTypeEnum } from "../../types/applicationImageType";
import { useSetPageTitle } from "../../hooks/useSetPageTitle";
import { extractErrorMessage, logError } from "../../utils/errorHandler";
import { GREEN_TEAL } from "../../constants/ColorConstants";
import { getApiBase } from "../../utils/envConfig";
import { PAGE_TITLES } from "../../constants/pageTitles";
import { useTheme } from '@mui/material/styles';
import { useSnackbar } from "../../contexts/SnackbarContext";
const ITEMS_PER_PAGE_OPTIONS = [5, 10, 20, 50];

interface OutletForm {
    Name: string;
    Code: string;
    Description: string;
    IconImageUrl: string;
    BackgroundImageUrl: string;
    PropertyIds: number[];
    IsActive: boolean;
    MainColor: string;
}

const defaultForm = (): OutletForm => ({
    Name: "", Code: "", Description: "", IconImageUrl: "", BackgroundImageUrl: "", PropertyIds: [], IsActive: true, MainColor: "",
});

export default function AdminOutletPage() {
    useSetPageTitle(PAGE_TITLES.OUTLET_MANAGEMENT);
    const muiTheme = useTheme();
    const { showSnackbar } = useSnackbar();
    const [outlets, setOutlets] = useState<OutletResponse[]>([]);
    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [outletImages, setOutletImages] = useState<ApplicationImageResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState("");
    const debouncedSearch = useDebounce(searchTerm, 300);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(10);

    // Dialog
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");
    const [dialogLoading, setDialogLoading] = useState(false);
    const [editingOutlet, setEditingOutlet] = useState<OutletResponse | null>(null);
    const [form, setForm] = useState<OutletForm>(defaultForm());

    // Delete
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [deletingOutlet, setDeletingOutlet] = useState<OutletResponse | null>(null);
    const [deleteLoading, setDeleteLoading] = useState(false);

    const loadData = async () => {
        try {
            setLoading(true);
            setError(null);
            const [outletData, propertyData, imgData] = await Promise.all([
                outletService.getListAsync(),
                propertyService.getListAsync(),
                applicationImageService.getByTypeAsync(ImageTypeEnum.Outlet),
            ]);
            setOutlets(outletData);
            setProperties(propertyData);
            setOutletImages(imgData);
        } catch (err: unknown) {
            logError("AdminOutletPage.loadData", err);
            setError(extractErrorMessage(err, "Failed to load outlets"));
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { loadData(); }, []);

    const filtered = useMemo(() => {
        const q = debouncedSearch.toLowerCase();
        return outlets.filter(
            (o) => o.name.toLowerCase().includes(q) || o.code.toLowerCase().includes(q) || o.description.toLowerCase().includes(q),
        );
    }, [outlets, debouncedSearch]);

    const totalPages = Math.ceil(filtered.length / itemsPerPage);
    const paginated = useMemo(() => {
        const start = (currentPage - 1) * itemsPerPage;
        return filtered.slice(start, start + itemsPerPage);
    }, [filtered, currentPage, itemsPerPage]);

    useEffect(() => { setCurrentPage(1); }, [debouncedSearch, itemsPerPage]);

    // ── Dialog ────────────────────────────────────────────────────────────────

    const handleOpenCreate = () => {
        setDialogMode("create");
        setEditingOutlet(null);
        setForm(defaultForm());
        setDialogOpen(true);
    };

    const handleOpenEdit = (outlet: OutletResponse) => {
        setDialogMode("edit");
        setEditingOutlet(outlet);
        setForm({
            Name: outlet.name,
            Code: outlet.code,
            Description: outlet.description,
            IconImageUrl: outlet.iconImageUrl,
            PropertyIds: outlet.properties.map((p) => p.id),
            IsActive: outlet.isActive,
            BackgroundImageUrl: outlet.backgroundImageUrl || "",
            MainColor: outlet.mainColor || "",
        });
        setDialogOpen(true);
    };

    const handleCloseDialog = () => { if (!dialogLoading) setDialogOpen(false); };

    const handleSubmit = async () => {
        if (!form.Name.trim()) return;
        try {
            setDialogLoading(true);
            if (dialogMode === "create") {
                const req: CreateOutletRequest = {
                    Name: form.Name, Code: form.Code, Description: form.Description,
                    IconImageUrl: form.IconImageUrl,
                    BackgroundImageUrl: form.BackgroundImageUrl,
                    PropertyIds: form.PropertyIds,
                    MainColor: form.MainColor,
                };
                await outletService.createAsync(req);
                showSnackbar("Outlet created successfully.", "success");
            } else if (editingOutlet) {
                const req: UpdateOutletRequest = {
                    Id: editingOutlet.id,
                    Name: form.Name, Code: form.Code,
                    Description: form.Description,
                    IconImageUrl: form.IconImageUrl,
                    BackgroundImageUrl: form.BackgroundImageUrl,
                    PropertyIds: form.PropertyIds,
                    IsActive: form.IsActive,
                    MainColor: form.MainColor,
                };
                await outletService.updateAsync(req);
                showSnackbar("Outlet updated successfully.", "success");
            }
            setDialogOpen(false);
            await loadData();
        } catch (err: unknown) {
            logError("AdminOutletPage.handleSubmit", err);
            showSnackbar(extractErrorMessage(err, "Failed to save outlet"), "error");
        } finally {
            setDialogLoading(false);
        }
    };

    // ── Delete ────────────────────────────────────────────────────────────────

    const handleOpenDelete = (outlet: OutletResponse) => { setDeletingOutlet(outlet); setDeleteDialogOpen(true); };
    const handleCloseDelete = () => { if (!deleteLoading) setDeleteDialogOpen(false); };

    const handleDelete = async () => {
        if (!deletingOutlet) return;
        try {
            setDeleteLoading(true);
            await outletService.deleteAsync(deletingOutlet.id);
            showSnackbar("Outlet deleted successfully.", "success");
            setDeleteDialogOpen(false);
            await loadData();
        } catch (err: unknown) {
            logError("AdminOutletPage.handleDelete", err);
            showSnackbar(extractErrorMessage(err, "Failed to delete outlet"), "error");
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
                        <OutletIcon color="primary" sx={{ fontSize: 28 }} />
                        <Typography variant="body2" color="text.secondary">
                            Manage outlets where your products are available. You can create, edit or delete outlets as needed.
                        </Typography>
                    </Box>
                    <Button variant="contained" startIcon={<AddIcon />} onClick={handleOpenCreate}>
                        Add Outlet
                    </Button>
                </Box>

                {/* Filters */}
                <Card sx={{ mb: 2 }}>
                    <CardContent sx={{ pb: "12px !important" }}>
                        <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap", alignItems: "center" }}>
                            <TextField
                                size="small"
                                placeholder="Search by name, code or description..."
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
                                <Select label="Per page" value={itemsPerPage} onChange={(e) => setItemsPerPage(Number(e.target.value))}>
                                    {ITEMS_PER_PAGE_OPTIONS.map((n) => <MenuItem key={n} value={n}>{n}</MenuItem>)}
                                </Select>
                            </FormControl>
                            <Tooltip title="Refresh">
                                <IconButton onClick={loadData} disabled={loading}><RefreshIcon /></IconButton>
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
                                    <TableCell>MAIN COLOR</TableCell>
                                    <TableCell>ICON</TableCell>
                                    <TableCell>BACKGROUND</TableCell>
                                    {/* <TableCell>AVAILABLE AT</TableCell> */}
                                    <TableCell width={90}>STATUS</TableCell>
                                    <TableCell width={100} align="center">ACTIONS</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {loading ? (
                                    Array.from({ length: 5 }).map((_, i) => (
                                        <TableRow key={i}>{Array.from({ length: 7 }).map((__, j) => <TableCell key={j}><Skeleton /></TableCell>)}</TableRow>
                                    ))
                                ) : error ? (
                                    <TableRow><TableCell colSpan={7}><Alert severity="error">{error}</Alert></TableCell></TableRow>
                                ) : paginated.length === 0 ? (
                                    <TableRow>
                                        <TableCell colSpan={7} align="center">
                                            <Typography color="text.secondary" py={3}>No outlets found.</Typography>
                                        </TableCell>
                                    </TableRow>
                                ) : (
                                    paginated.map((outlet, idx) => (
                                        <TableRow key={outlet.id} hover>
                                            <TableCell>{(currentPage - 1) * itemsPerPage + idx + 1}</TableCell>
                                            <TableCell><Typography fontWeight={600}>{outlet.name}</Typography></TableCell>
                                            <TableCell>
                                                <Chip size="small" label={outlet.code || "—"} variant="outlined"
                                                    sx={{ borderColor: outlet.mainColor, color: outlet.mainColor }} />
                                            </TableCell>
                                            <TableCell>
                                                <Box sx={{ width: 20, height: 20, bgcolor: outlet.mainColor || "transparent", borderRadius: 0.5 }} >
                                                    <span style=
                                                        {{
                                                            display: "block",
                                                            width: "100%", height: "100%",
                                                            border: outlet.mainColor ? "none" : `1px solid ${muiTheme.palette.divider}`,
                                                            padding: "0 25px 0 25px",
                                                        }}>
                                                        {outlet.mainColor}
                                                    </span>
                                                </Box>
                                            </TableCell>
                                            <TableCell>
                                                {(outlet.iconImageUrl) ? (
                                                    <Box
                                                        component="img"
                                                        src={getApiBase() + (outlet.iconImageUrl)}
                                                        alt={outlet.name}
                                                        sx={{ width: 50, height: 60, objectFit: "contain", borderRadius: 1 }}
                                                        onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }}
                                                    />
                                                ) : "—"}
                                            </TableCell>
                                            <TableCell>
                                                {(outlet.backgroundImageUrl) ? (
                                                    <Box
                                                        component="img"
                                                        src={getApiBase() + (outlet.backgroundImageUrl)}
                                                        alt={outlet.name}
                                                        sx={{ width: 50, height: 60, objectFit: "contain", borderRadius: 1 }}
                                                        onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }}
                                                    />
                                                ) : "—"}
                                            </TableCell>
                                            {/* <TableCell>
                                                <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                                                    {outlet.properties.length === 0
                                                        ? <Typography variant="body2" color="text.secondary">—</Typography>
                                                        : outlet.properties.map((p) => (
                                                            <Chip
                                                                variant="outlined"
                                                                key={p.id}
                                                                size="small"
                                                                label={`${p.code} - ${p.name}${p.isPrimaryOutlet ? " (Main outlet)" : ""}`}
                                                                sx={{
                                                                    borderColor: p.color,
                                                                    color: p.color,
                                                                    fontWeight: 700,
                                                                    fontSize: "0.8rem"
                                                                }}
                                                            />
                                                        ))}
                                                </Box>
                                            </TableCell> */}
                                            < TableCell >
                                                <Chip size="small" label={outlet.isActive ? "Active" : "Inactive"} color={outlet.isActive ? "success" : "error"} />
                                            </TableCell>
                                            <TableCell align="center">
                                                <Tooltip title="Edit">
                                                    <IconButton size="small" onClick={() => handleOpenEdit(outlet)}><EditIcon fontSize="small" /></IconButton>
                                                </Tooltip>
                                                <Tooltip title="Delete">
                                                    <IconButton size="small" color="error" onClick={() => handleOpenDelete(outlet)}><DeleteIcon fontSize="small" /></IconButton>
                                                </Tooltip>
                                            </TableCell>
                                        </TableRow>
                                    ))
                                )}
                            </TableBody>
                        </Table>
                    </TableContainer>
                    {!loading && totalPages > 1 && (
                        <Box sx={{ display: "flex", justifyContent: "center", p: 2 }}>
                            <Pagination count={totalPages} page={currentPage} onChange={(_, p) => setCurrentPage(p)} color="primary" size="small" />
                        </Box>
                    )}
                </Card>

                {/* Create / Edit Dialog */}
                <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
                    <DialogTitle>{dialogMode === "create" ? "Create Outlet" : "Edit Outlet"}</DialogTitle>
                    <DialogContent dividers>
                        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, pt: 1 }}>
                            <TextField
                                label="Name *"
                                value={form.Name}
                                onChange={(e) => setForm({ ...form, Name: e.target.value })}
                                fullWidth size="small" autoFocus
                            />
                            <TextField
                                label="Code"
                                value={form.Code}
                                onChange={(e) => setForm({ ...form, Code: e.target.value })}
                                fullWidth size="small"
                            />
                            <TextField
                                label="Description"
                                value={form.Description}
                                onChange={(e) => setForm({ ...form, Description: e.target.value })}
                                fullWidth size="small" multiline rows={2}
                            />

                            <TextField
                                label="Main Color"
                                value={form.MainColor}
                                onChange={(e) => setForm({ ...form, MainColor: e.target.value })}
                                fullWidth size="small"
                                type="color"
                                InputLabelProps={{ shrink: true }}
                            />

                            {/* Icon URL – dropdown from ApplicationImage (type = Outlet) */}
                            <FormControl size="small" fullWidth>
                                <InputLabel>Icon Image</InputLabel>
                                <Select
                                    label="Icon Image"
                                    value={form.IconImageUrl}
                                    onChange={(e) => setForm({ ...form, IconImageUrl: String(e.target.value) })}
                                    renderValue={(selected) => {
                                        if (!selected) return <em>None</em>;
                                        const img = outletImages.find((i) => i.fileUrl === selected);
                                        return (
                                            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                                <Box component="img" src={getApiBase() + selected} sx={{ width: 24, height: 24, objectFit: "cover", borderRadius: 0.5 }}
                                                    onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }} />
                                                <Typography variant="body2"> {img?.name ?? selected}</Typography>
                                            </Box>
                                        );
                                    }}
                                >
                                    <MenuItem value=""><em>None</em></MenuItem>
                                    {outletImages.map((img) => (
                                        <MenuItem key={img.id} value={img.fileUrl}>
                                            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                                <Box component="img" src={getApiBase() + img.fileUrl} alt={img.name}
                                                    sx={{ width: 32, height: 32, objectFit: "cover", borderRadius: 0.5, border: "1px solid #ddd" }}
                                                    onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }} />
                                                <Typography variant="body2">{img.name}</Typography>
                                            </Box>
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            {outletImages.length === 0 && (
                                <Typography variant="caption" color="text.secondary">
                                    No outlet images available. Upload images with type "Outlet Image" in Application Image Management.
                                </Typography>
                            )}

                            {/* Background URL – dropdown from ApplicationImage (type = Outlet) */}
                            <FormControl size="small" fullWidth>
                                <InputLabel>Background Image</InputLabel>
                                <Select
                                    label="Background Image"
                                    value={form.BackgroundImageUrl}
                                    onChange={(e) => setForm({ ...form, BackgroundImageUrl: String(e.target.value) })}
                                    renderValue={(selected) => {
                                        if (!selected) return <em>None</em>;
                                        const img = outletImages.find((i) => i.fileUrl === selected);
                                        return (
                                            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                                <Box component="img" src={getApiBase() + selected} sx={{ width: 24, height: 24, objectFit: "cover", borderRadius: 0.5 }}
                                                    onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }} />
                                                <Typography variant="body2"> {img?.name ?? selected}</Typography>
                                            </Box>
                                        );
                                    }}
                                >
                                    <MenuItem value=""><em>None</em></MenuItem>
                                    {outletImages.map((img) => (
                                        <MenuItem key={img.id} value={img.fileUrl}>
                                            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                                <Box component="img" src={getApiBase() + img.fileUrl} alt={img.name}
                                                    sx={{ width: 32, height: 32, objectFit: "cover", borderRadius: 0.5, border: "1px solid #ddd" }}
                                                    onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }} />
                                                <Typography variant="body2">{img.name}</Typography>
                                            </Box>
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            {outletImages.length === 0 && (
                                <Typography variant="caption" color="text.secondary">
                                    No outlet images available. Upload images with type "Outlet Image" in Application Image Management.
                                </Typography>
                            )}

                            {/* Properties multi-select */}
                            <FormControl size="small" fullWidth>
                                <InputLabel>Properties</InputLabel>
                                <Select
                                    multiple
                                    label="Properties"
                                    value={form.PropertyIds}
                                    onChange={(e) => {
                                        const val = e.target.value;
                                        setForm({ ...form, PropertyIds: (typeof val === "string" ? val.split(",").map(Number) : val as number[]) });
                                    }}
                                    input={<OutlinedInput label="Properties" />}
                                    renderValue={(selected) =>
                                        (selected as number[])
                                            .map((id) => properties.find((p) => p.id === id)?.name ?? `#${id}`)
                                            .join(", ")
                                    }
                                >
                                    {properties.map((p) => (
                                        <MenuItem key={p.id} value={p.id}>
                                            <Checkbox checked={form.PropertyIds.includes(p.id)} size="small" />
                                            <ListItemText primary={p.name} />
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>

                            {dialogMode === "edit" && (
                                <FormControlLabel
                                    control={<Switch checked={form.IsActive} onChange={(e) => setForm({ ...form, IsActive: e.target.checked })} color="success" />}
                                    label="Active"
                                />
                            )}
                        </Box>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCloseDialog} disabled={dialogLoading}>Cancel</Button>
                        <Button variant="contained" onClick={handleSubmit} disabled={dialogLoading || !form.Name.trim()}>
                            {dialogLoading ? "Saving..." : dialogMode === "create" ? "Create" : "Update"}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* Delete Dialog */}
                <Dialog open={deleteDialogOpen} onClose={handleCloseDelete}>
                    <DialogTitle>Confirm Delete</DialogTitle>
                    <DialogContent>
                        <Typography>Are you sure you want to delete outlet <strong>{deletingOutlet?.name}</strong>?</Typography>
                        <Alert severity="warning" sx={{ mt: 2 }}>This action cannot be undone.</Alert>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCloseDelete} disabled={deleteLoading}>Cancel</Button>
                        <Button onClick={handleDelete} color="error" variant="contained" disabled={deleteLoading}>
                            {deleteLoading ? "Deleting..." : "Delete"}
                        </Button>
                    </DialogActions>
                </Dialog>
            </Box >
        </AdminLayout >
    );
}
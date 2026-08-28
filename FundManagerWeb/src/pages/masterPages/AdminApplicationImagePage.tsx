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
} from "@mui/material";
import {
    Search as SearchIcon,
    Refresh as RefreshIcon,
    Add as AddIcon,
    Edit as EditIcon,
    Delete as DeleteIcon,
    Image as ImageIcon,
    CloudUpload as UploadIcon,
    ZoomIn as ZoomInIcon,
    Download as DownloadIcon,
    Close as CloseIcon,
} from "@mui/icons-material";
import { useState, useEffect, useMemo, useRef } from "react";
import AdminLayout from "../../components/layout/AdminLayout";
import { applicationImageService } from "../../services/applicationImageService";
import { outletService } from "../../services/outletService";
import { propertyService } from "../../services/propertyService";
import { useDebounce } from "../../hooks/useDebounce";
import { useSetPageTitle } from "../../hooks/useSetPageTitle";
import { extractErrorMessage, logError } from "../../utils/errorHandler";
import { getApiBase } from "../../utils/envConfig";
import { PAGE_TITLES } from "../../constants/pageTitles";
import { IMAGE_TYPE_LABELS, ImageTypeEnum, SLIDER_TYPES_REQUIRE_OUTLET, SLIDER_TYPES_REQUIRE_PROPERTY, type ApplicationImageResponse, type CreateApplicationImageFormData, type UpdateApplicationImageFormData } from "../../types/applicationImageType";
import type { OutletResponse } from "../../types/outletType";
import type { PropertyResponse } from "../../types/propertyType";
import { useTheme } from '@mui/material/styles';
const ITEMS_PER_PAGE_OPTIONS = [5, 10, 20, 50];
const ALL_IMAGE_TYPES = Object.values(ImageTypeEnum).filter((v) => typeof v === "number") as ImageTypeEnum[];
import { useSnackbar } from "../../contexts/SnackbarContext";

interface FormState {
    Name: string;
    Description: string;
    Type: ImageTypeEnum;
    PropertyId: number | null;
    OutletId: number | null;
    IsActive: boolean;
    /** Selected file for upload (null = keep existing on edit) */
    File: File | null;
    /** Preview URL for the selected file or existing fileUrl */
    previewUrl: string;
}

const defaultForm = (): FormState => ({
    Name: "", Description: "", Type: ImageTypeEnum.Other,
    PropertyId: null, OutletId: null, IsActive: true, File: null, previewUrl: "",
});

export default function AdminApplicationImagePage() {
    useSetPageTitle(PAGE_TITLES.APPLICATION_IMAGE_MANAGEMENT);

    const { showSnackbar } = useSnackbar();

    const muiTheme = useTheme();
    const [images, setImages] = useState<ApplicationImageResponse[]>([]);
    const [outlets, setOutlets] = useState<OutletResponse[]>([]);
    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState("");
    const debouncedSearch = useDebounce(searchTerm, 300);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(10);
    const [filterType, setFilterType] = useState<ImageTypeEnum | "">("");

    // Dialog
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");
    const [dialogLoading, setDialogLoading] = useState(false);
    const [editingImage, setEditingImage] = useState<ApplicationImageResponse | null>(null);
    const [form, setForm] = useState<FormState>(defaultForm());
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Image preview dialog
    const [previewDialogOpen, setPreviewDialogOpen] = useState(false);
    const [previewDialogUrl, setPreviewDialogUrl] = useState("");
    const [previewDialogName, setPreviewDialogName] = useState("");

    const handleOpenPreview = (url: string, name: string) => {
        setPreviewDialogUrl(url);
        setPreviewDialogName(name);
        setPreviewDialogOpen(true);
    };

    const handleDownloadPreview = async () => {
        try {
            const response = await fetch(previewDialogUrl);
            const blob = await response.blob();
            const blobUrl = URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = blobUrl;
            const ext = previewDialogUrl.split(".").pop()?.split("?")[0] || "jpg";
            a.download = `${previewDialogName}.${ext}`;
            a.click();
            URL.revokeObjectURL(blobUrl);
        } catch {
            // fallback: open in new tab
            window.open(previewDialogUrl, "_blank");
        }
    };

    // Delete
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [deletingImage, setDeletingImage] = useState<ApplicationImageResponse | null>(null);
    const [deleteLoading, setDeleteLoading] = useState(false);

    const loadData = async () => {
        try {
            setLoading(true);
            setError(null);
            const [imgData, outletData, propertyData] = await Promise.all([
                applicationImageService.getListAsync(),
                outletService.getListAsync(),
                propertyService.getListAsync(),
            ]);
            setImages(imgData);
            setOutlets(outletData);
            setProperties(propertyData);
        } catch (err: unknown) {
            logError("AdminApplicationImagePage.loadData", err);
            setError(extractErrorMessage(err, "Failed to load images"));
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { loadData(); }, []);

    const filtered = useMemo(() => {
        const q = debouncedSearch.toLowerCase();
        return images.filter((img) => {
            const matchType = filterType === "" || img.type === filterType;
            const matchSearch = img.name.toLowerCase().includes(q) || img.typeName.toLowerCase().includes(q);
            return matchType && matchSearch;
        });
    }, [images, debouncedSearch, filterType]);

    const totalPages = Math.ceil(filtered.length / itemsPerPage);
    const paginated = useMemo(() => {
        const start = (currentPage - 1) * itemsPerPage;
        return filtered.slice(start, start + itemsPerPage);
    }, [filtered, currentPage, itemsPerPage]);

    useEffect(() => { setCurrentPage(1); }, [debouncedSearch, itemsPerPage, filterType]);

    const getPropertyName = (id: number | null) =>
        id ? (properties.find((p) => p.id === id)?.name ?? `#${id}`) : "—";
    const getOutletName = (id: number | null) =>
        id ? (outlets.find((o) => o.id === id)?.name ?? `#${id}`) : "—";

    const getTypeChipColor = (type: ImageTypeEnum): "primary" | "secondary" | "success" | "warning" | "info" | "default" => {
        if (type === ImageTypeEnum.SliderHotel) return "primary";
        if (type === ImageTypeEnum.SliderOutlet) return "secondary";
        if (type === ImageTypeEnum.Slider) return "info";
        if (type === ImageTypeEnum.Logo) return "success";
        if (type === ImageTypeEnum.Outlet) return "warning";
        return "default";
    };

    const isFormValid = () => {
        if (!form.Name.trim()) return false;
        if (!form.Description.trim()) return false;
        if (dialogMode === "create" && !form.File) return false;
        if (SLIDER_TYPES_REQUIRE_PROPERTY.includes(form.Type) && !form.PropertyId) return false;
        if (SLIDER_TYPES_REQUIRE_OUTLET.includes(form.Type) && !form.PropertyId) return false;
        if (SLIDER_TYPES_REQUIRE_OUTLET.includes(form.Type) && !form.OutletId) return false;
        return true;
    };

    // ── File pick ─────────────────────────────────────────────────────────────

    const handleFilePick = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;
        const url = URL.createObjectURL(file);
        setForm((prev) => ({ ...prev, File: file, previewUrl: url }));
    };

    // ── Dialog ────────────────────────────────────────────────────────────────

    const handleOpenCreate = () => {
        setDialogMode("create");
        setEditingImage(null);
        setForm(defaultForm());
        setDialogOpen(true);
    };

    const handleOpenEdit = (img: ApplicationImageResponse) => {
        setDialogMode("edit");
        setEditingImage(img);
        setForm({
            Name: img.name, Description: img.description,
            Type: img.type, PropertyId: img.propertyId, OutletId: img.outletId,
            IsActive: img.isActive, File: null, previewUrl: img.fileUrl,
        });
        setDialogOpen(true);
    };

    const handleCloseDialog = () => {
        if (!dialogLoading) {
            setDialogOpen(false);
            // Revoke blob URL to avoid memory leak
            if (form.previewUrl.startsWith("blob:")) URL.revokeObjectURL(form.previewUrl);
        }
    };

    const handleSubmit = async () => {
        if (!isFormValid()) return;
        try {
            setDialogLoading(true);
            if (dialogMode === "create") {
                const req: CreateApplicationImageFormData = {
                    Name: form.Name,
                    Description: form.Description,
                    File: form.File!,
                    Type: form.Type,
                    PropertyId: form.PropertyId,
                    OutletId: form.OutletId,
                };
                await applicationImageService.createAsync(req);
                showSnackbar("Image created successfully.", "success");
            } else if (editingImage) {
                const req: UpdateApplicationImageFormData = {
                    Id: editingImage.id,
                    Name: form.Name,
                    Description: form.Description,
                    File: form.File ?? undefined,
                    Type: form.Type,
                    PropertyId: form.PropertyId,
                    OutletId: form.OutletId,
                    IsActive: form.IsActive,
                };
                await applicationImageService.updateAsync(req);
                showSnackbar("Image updated successfully.", "success");
            }
            setDialogOpen(false);
            await loadData();
        } catch (err: unknown) {
            logError("AdminApplicationImagePage.handleSubmit", err);
            showSnackbar(extractErrorMessage(err, "Failed to save image"), "error");
        } finally {
            setDialogLoading(false);
        }
    };

    // ── Delete ────────────────────────────────────────────────────────────────

    const handleOpenDelete = (img: ApplicationImageResponse) => { setDeletingImage(img); setDeleteDialogOpen(true); };
    const handleCloseDelete = () => { if (!deleteLoading) setDeleteDialogOpen(false); };

    const handleDelete = async () => {
        if (!deletingImage) return;
        try {
            setDeleteLoading(true);
            await applicationImageService.deleteAsync(deletingImage.id);
            showSnackbar("Image deleted successfully.", "success");
            setDeleteDialogOpen(false);
            await loadData();
        } catch (err: unknown) {
            logError("AdminApplicationImagePage.handleDelete", err);
            showSnackbar(extractErrorMessage(err, "Failed to delete image"), "error");
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
                        <ImageIcon color="primary" sx={{ fontSize: 28 }} />
                        <Typography variant="body2" color="text.secondary">
                            Manage all images for the application (Slider – Hotel, Slider – Outlet, Logo, ...).
                        </Typography>
                    </Box>
                    <Button variant="contained" startIcon={<AddIcon />} onClick={handleOpenCreate}>
                        Add Image
                    </Button>
                </Box>

                {/* Filters */}
                <Card sx={{ mb: 2 }}>
                    <CardContent sx={{ pb: "12px !important" }}>
                        <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap", alignItems: "center" }}>
                            <TextField
                                size="small"
                                placeholder="Search by name or type..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                sx={{ minWidth: 300 }}
                                InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> }}
                            />
                            <FormControl size="small" sx={{ minWidth: 160 }}>
                                <InputLabel>Image Type</InputLabel>
                                <Select
                                    label="Image Type"
                                    value={filterType}
                                    onChange={(e) => { const v = e.target.value; setFilterType(String(v) === "" ? "" : Number(v) as ImageTypeEnum); }}
                                >
                                    <MenuItem value=""><em>All types</em></MenuItem>
                                    {ALL_IMAGE_TYPES.map((t) => (
                                        <MenuItem key={t} value={t}>{IMAGE_TYPE_LABELS[t]}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
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
                                    <TableCell>TYPE</TableCell>
                                    <TableCell>PROPERTY</TableCell>
                                    <TableCell>OUTLET</TableCell>
                                    <TableCell>PREVIEW</TableCell>
                                    <TableCell width={90}>STATUS</TableCell>
                                    <TableCell width={100} align="center">ACTIONS</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {loading ? (
                                    Array.from({ length: 5 }).map((_, i) => (
                                        <TableRow key={i}>{Array.from({ length: 8 }).map((__, j) => <TableCell key={j}><Skeleton /></TableCell>)}</TableRow>
                                    ))
                                ) : error ? (
                                    <TableRow><TableCell colSpan={8}><Alert severity="error">{error}</Alert></TableCell></TableRow>
                                ) : paginated.length === 0 ? (
                                    <TableRow>
                                        <TableCell colSpan={8} align="center">
                                            <Typography color="text.secondary" py={3}>No images found.</Typography>
                                        </TableCell>
                                    </TableRow>
                                ) : (
                                    paginated.map((img, idx) => (
                                        <TableRow key={img.id} hover>
                                            <TableCell>{(currentPage - 1) * itemsPerPage + idx + 1}</TableCell>
                                            <TableCell><Typography fontWeight={600}>{img.name}</Typography></TableCell>
                                            <TableCell><Chip size="small" label={img.typeName} color={getTypeChipColor(img.type)} variant="outlined" /></TableCell>
                                            <TableCell><Typography variant="body2">{getPropertyName(img.propertyId)}</Typography></TableCell>
                                            <TableCell><Typography variant="body2">{getOutletName(img.outletId)}</Typography></TableCell>
                                            <TableCell>
                                                {img.fileUrl ? (
                                                    <Tooltip title="Click to preview">
                                                        <Box
                                                            component="img"
                                                            src={getApiBase() + img.fileUrl}
                                                            alt={img.name}
                                                            onClick={() => handleOpenPreview(getApiBase() + img.fileUrl, img.name)}
                                                            sx={{
                                                                width: 48, height: 36, objectFit: "contain",
                                                                borderRadius: 1, border: "1px solid #ddd",
                                                                cursor: "pointer",
                                                                transition: "transform 0.2s, box-shadow 0.2s",
                                                                "&:hover": { transform: "scale(1.1)", boxShadow: "0 4px 12px rgba(0,0,0,0.25)" },
                                                            }}
                                                            onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }}
                                                        />
                                                    </Tooltip>
                                                ) : "—"}
                                            </TableCell>
                                            <TableCell>
                                                <Chip size="small" label={img.isActive ? "Active" : "Inactive"} color={img.isActive ? "success" : "error"} />
                                            </TableCell>
                                            <TableCell align="center">
                                                <Tooltip title="Edit">
                                                    <IconButton size="small" onClick={() => handleOpenEdit(img)}><EditIcon fontSize="small" /></IconButton>
                                                </Tooltip>
                                                <Tooltip title="Delete">
                                                    <IconButton size="small" color="error" onClick={() => handleOpenDelete(img)}><DeleteIcon fontSize="small" /></IconButton>
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
                    <DialogTitle>{dialogMode === "create" ? "Add Image" : "Edit Image"}</DialogTitle>
                    <DialogContent dividers>
                        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, pt: 1 }}>
                            <TextField
                                label="Name *"
                                value={form.Name}
                                onChange={(e) => setForm({ ...form, Name: e.target.value })}
                                fullWidth size="small" autoFocus
                            />
                            <FormControl size="small" fullWidth required>
                                <InputLabel>Image Type *</InputLabel>
                                <Select
                                    label="Image Type *"
                                    value={form.Type}
                                    onChange={(e) => setForm({ ...form, Type: Number(e.target.value) as ImageTypeEnum, PropertyId: null, OutletId: null })}
                                >
                                    {ALL_IMAGE_TYPES.map((t) => (
                                        <MenuItem key={t} value={t}>{IMAGE_TYPE_LABELS[t]}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>

                            {/* Slider / SliderHotel → require PropertyId */}
                            {SLIDER_TYPES_REQUIRE_PROPERTY.includes(form.Type) && (
                                <FormControl size="small" fullWidth required>
                                    <InputLabel>Property * (required for {IMAGE_TYPE_LABELS[form.Type]})</InputLabel>
                                    <Select
                                        label={`Property * (required for ${IMAGE_TYPE_LABELS[form.Type]})`}
                                        value={form.PropertyId ?? ""}
                                        onChange={(e) => setForm({ ...form, PropertyId: String(e.target.value) === "" ? null : Number(e.target.value) })}
                                    >
                                        <MenuItem value=""><em>Select a property</em></MenuItem>
                                        {properties.map((p) => <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>)}
                                    </Select>
                                </FormControl>
                            )}

                            {/* Outlet / SliderOutlet → require both PropertyId and OutletId */}
                            {SLIDER_TYPES_REQUIRE_OUTLET.includes(form.Type) && (
                                <>
                                    <FormControl size="small" fullWidth required>
                                        <InputLabel>Property * (required for {IMAGE_TYPE_LABELS[form.Type]})</InputLabel>
                                        <Select
                                            label={`Property * (required for ${IMAGE_TYPE_LABELS[form.Type]})`}
                                            value={form.PropertyId ?? ""}
                                            onChange={(e) => setForm({ ...form, PropertyId: String(e.target.value) === "" ? null : Number(e.target.value) })}
                                        >
                                            <MenuItem value=""><em>Select a property</em></MenuItem>
                                            {properties.map((p) => <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>)}
                                        </Select>
                                    </FormControl>
                                    <FormControl size="small" fullWidth required>
                                        <InputLabel>Outlet * (required for {IMAGE_TYPE_LABELS[form.Type]})</InputLabel>
                                        <Select
                                            label={`Outlet * (required for ${IMAGE_TYPE_LABELS[form.Type]})`}
                                            value={form.OutletId ?? ""}
                                            onChange={(e) => setForm({ ...form, OutletId: String(e.target.value) === "" ? null : Number(e.target.value) })}
                                        >
                                            <MenuItem value=""><em>Select an outlet</em></MenuItem>
                                            {outlets.map((o) => <MenuItem key={o.id} value={o.id}>{o.name}</MenuItem>)}
                                        </Select>
                                    </FormControl>
                                </>
                            )}

                            {/* File upload */}
                            <Box>
                                <input
                                    ref={fileInputRef}
                                    type="file"
                                    accept="image/*"
                                    style={{ display: "none" }}
                                    onChange={handleFilePick}
                                />
                                <Button
                                    variant="outlined"
                                    startIcon={<UploadIcon />}
                                    onClick={() => fileInputRef.current?.click()}
                                    size="small"
                                    fullWidth
                                >
                                    {dialogMode === "create"
                                        ? (form.File ? form.File.name : "Choose image file *")
                                        : (form.File ? form.File.name : "Replace image (optional)")}
                                </Button>
                                {dialogMode === "create" && !form.File && (
                                    <Typography variant="caption" color="error">Image file is required.</Typography>
                                )}
                            </Box>

                            {/* Preview */}
                            {form.previewUrl && (
                                <Box sx={{ textAlign: "center" }}>
                                    <Box
                                        component="img"
                                        src={form.previewUrl}
                                        alt="preview"
                                        sx={{ maxHeight: 160, maxWidth: "100%", borderRadius: 1, border: "1px solid #ddd", objectFit: "contain" }}
                                        onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }}
                                    />
                                </Box>
                            )}

                            <TextField
                                label="Description *"
                                value={form.Description}
                                onChange={(e) => setForm({ ...form, Description: e.target.value })}
                                fullWidth size="small" multiline rows={2}
                                error={form.Description.trim() === ""}
                                helperText={form.Description.trim() === "" ? "Description is required." : ""}
                            />
                            {dialogMode === "edit" && (
                                <FormControlLabel
                                    control={<Switch checked={form.IsActive} onChange={(e) => setForm({ ...form, IsActive: e.target.checked })} color="success" />}
                                    label="Active"
                                />
                            )}

                            {/* Validation hints */}
                            {SLIDER_TYPES_REQUIRE_PROPERTY.includes(form.Type) && !form.PropertyId && (
                                <Alert severity="warning" sx={{ py: 0.5 }}>PropertyId is required for <strong>{IMAGE_TYPE_LABELS[form.Type]}</strong>.</Alert>
                            )}
                            {SLIDER_TYPES_REQUIRE_OUTLET.includes(form.Type) && !form.PropertyId && (
                                <Alert severity="warning" sx={{ py: 0.5 }}>Property is required for <strong>{IMAGE_TYPE_LABELS[form.Type]}</strong>.</Alert>
                            )}
                            {SLIDER_TYPES_REQUIRE_OUTLET.includes(form.Type) && !form.OutletId && (
                                <Alert severity="warning" sx={{ py: 0.5 }}>Outlet is required for <strong>{IMAGE_TYPE_LABELS[form.Type]}</strong>.</Alert>
                            )}
                        </Box>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCloseDialog} disabled={dialogLoading}>Cancel</Button>
                        <Button variant="contained" onClick={handleSubmit} disabled={dialogLoading || !isFormValid()}>
                            {dialogLoading ? "Saving..." : dialogMode === "create" ? "Create" : "Update"}
                        </Button>
                    </DialogActions>
                </Dialog>

                {/* Image Preview Dialog */}
                <Dialog
                    open={previewDialogOpen}
                    onClose={() => setPreviewDialogOpen(false)}
                    maxWidth="lg"
                    fullWidth
                    PaperProps={{ sx: { borderRadius: 3, overflow: "hidden" } }}
                >
                    <DialogTitle sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", pb: 1 }}>
                        <Typography fontWeight={700} noWrap sx={{ flex: 1, mr: 2 }}>{previewDialogName}</Typography>
                        <Box sx={{ display: "flex", gap: 1 }}>
                            <Tooltip title="Download">
                                <IconButton onClick={handleDownloadPreview} color="primary" size="small"
                                    sx={{ bgcolor: "primary.main", color: "#fff", "&:hover": { bgcolor: "primary.dark" } }}>
                                    <DownloadIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Close">
                                <IconButton onClick={() => setPreviewDialogOpen(false)} size="small">
                                    <CloseIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                        </Box>
                    </DialogTitle>
                    <DialogContent sx={{ p: 0, display: "flex", justifyContent: "center", alignItems: "center", bgcolor: "#000", minHeight: 300 }}>
                        <Box
                            component="img"
                            src={previewDialogUrl}
                            alt={previewDialogName}
                            sx={{ maxWidth: "100%", maxHeight: "80vh", objectFit: "contain", display: "block" }}
                            onError={(e) => { (e.target as HTMLImageElement).alt = "Image failed to load"; }}
                        />
                    </DialogContent>
                </Dialog>

                {/* Delete Dialog */}
                <Dialog open={deleteDialogOpen} onClose={handleCloseDelete}>
                    <DialogTitle>Confirm Delete</DialogTitle>
                    <DialogContent>
                        <Typography>Are you sure you want to delete image <strong>{deletingImage?.name}</strong>?</Typography>
                        <Alert severity="warning" sx={{ mt: 2 }}>This action cannot be undone.</Alert>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCloseDelete} disabled={deleteLoading}>Cancel</Button>
                        <Button onClick={handleDelete} color="error" variant="contained" disabled={deleteLoading}>
                            {deleteLoading ? "Deleting..." : "Delete"}
                        </Button>
                    </DialogActions>
                </Dialog>
            </Box>
        </AdminLayout>
    );
}

import React, { useCallback, useEffect, useRef, useState } from 'react';
import AdminLayout from '../../components/layout/AdminLayout';
import {
    Box, Card, CardContent, Typography, Table, TableBody, TableCell, TableContainer,
    TableHead, TableRow, Button, Dialog, DialogTitle, DialogContent, DialogActions,
    IconButton, Stack, Paper, Chip, CircularProgress, TextField, InputAdornment,
    Pagination, Tooltip, Collapse, Divider, MenuItem, Select, FormControl, InputLabel,
    Autocomplete,
    List,
    ListItem,
    ListItemButton,
    ListItemIcon,
    ListItemText,
} from '@mui/material';

import {
    Search as SearchIcon,
    Refresh as RefreshIcon,
    Visibility as VisibilityIcon,
    Download as DownloadIcon,
    FileCopy as FileCopyIcon,
    ExpandMore as ExpandMoreIcon,
    ExpandLess as ExpandLessIcon,
    Article as ArticleIcon,
    Person as PersonIcon,
    Close as CloseIcon,
    FilePresent as FileIcon,
    PictureAsPdf as PDFIcon,
    Print as PrintIcon,
    Smartphone as PhoneIcon,
    Email as EmailIcon,
} from '@mui/icons-material';

import { useSetPageTitle } from '../../hooks/useSetPageTitle';
import { PAGE_TITLES } from '../../constants/pageTitles';
import { customerSignedService } from '../../services/customerSignedService';
import type {
    SignedCustomerRow,
    SignedDocumentRow,
} from '../../types/customerSignedType';
import { GREEN_TEAL } from '../../constants/ColorConstants';
import { api } from '../../services/commonApiService';
import type { OutletResponse } from '../../types/outletType';
import { useAppData } from '../../contexts/AppDataContext';
import { useTheme } from '@mui/material/styles';
import { getApiBase } from '../../utils/envConfig';
import { extractErrorMessage, logError } from '../../utils/errorHandler';
import { useSnackbar } from '../../contexts/SnackbarContext';
import { FormatUtcTime } from '../../utils/formatUtcTime';

const fmtDateTime = (iso: string) =>
    iso
        ? new Date(iso).toLocaleString('vi-VN', {
            day: '2-digit', month: '2-digit', year: 'numeric',
            hour: '2-digit', minute: '2-digit',
        })
        : '';

const PAGE_SIZE_OPTIONS = [10, 25, 50];
const CUSTOMER_TYPE_COLORS: Record<string, 'primary' | 'success' | 'default'> = {
    InHouse: 'success',
    WalkIn: 'primary',
};

function DetailDialog({
    row, open, onClose, onDuplicate,
}: {
    row: SignedCustomerRow | null;
    open: boolean;
    onClose: () => void;
    onDuplicate: (r: SignedCustomerRow) => void;
}) {
    const { showSnackbar } = useSnackbar();
    // Handle download
    // const handleViewDownload = (url: string, filename: string) => {
    //     if (!url) {
    //         showSnackbar('Document URL is not available', 'error');
    //         return;
    //     }
    //     try {
    //         const link = document.createElement('a');
    //         link.href = url.startsWith('http') ? url : `${getApiBase()}${url}`;
    //         link.setAttribute('download', filename);
    //         link.setAttribute('target', '_blank');
    //         document.body.appendChild(link);
    //         link.click();
    //         document.body.removeChild(link);
    //     } catch (err: unknown) {
    //         const errorMessage = extractErrorMessage(err, "Failed to download document. Please try again.");
    //         logError('AdminCustomerSignedPage.handleViewDownload', err);
    //         showSnackbar(errorMessage, "error");
    //     }
    // };

    const handleDownloadDocument = async (doc: SignedDocumentRow) => {
        if (!doc.fileUrl) {
            showSnackbar('Document URL is not available', 'error');
            return;
        }

        // Downlaod file when clicking the download button
        const link = document.createElement('a');
        const response = await fetch(doc.fileUrl.startsWith('http') ? doc.fileUrl : `${getApiBase()}${doc.fileUrl}`);
        const blob = await response.blob();
        link.href = URL.createObjectURL(blob);
        link.setAttribute('download', doc.fileName);
        link.setAttribute('target', '_blank');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        showSnackbar(`Downloading ${doc.fileName}`, 'success');
    }

    if (!row) return null;

    return (
        <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
            <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <PersonIcon sx={{ color: GREEN_TEAL }} />
                Guest Detail  {row.displayId}
            </DialogTitle>
            <DialogContent dividers>
                <Paper variant="outlined" sx={{ p: 2, mb: 2 }}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        GUEST INFORMATION
                    </Typography>
                    <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 1.5 }}>
                        {[
                            ['Full Name', row.customerName],
                            ['Room', row.roomNumber],
                            ['Language', row.language],
                            ['Outlet', row.outletName],
                            ['Signed At', fmtDateTime(row.signedAt)],
                        ].map(([label, value]) => (
                            <Box key={label as string}>
                                <Typography variant="caption" color="text.secondary">{label}</Typography>
                                <Typography fontWeight={600}>{value || ''}</Typography>
                            </Box>
                        ))}
                        <Box>
                            <Typography variant="caption" color="text.secondary">Customer Type</Typography>
                            <Box>
                                <Chip
                                    label={row.customerType || ''}
                                    size="small"
                                    color={row.customerType ? (CUSTOMER_TYPE_COLORS[row.customerType] ?? 'default') : 'default'}
                                />
                            </Box>
                        </Box>
                        {/* <Box>
                            <Typography variant="caption" color="text.secondary">Patron Category</Typography>
                            <Box>
                                {row.patronType
                                    ? <Chip label={row.patronType} size="small" sx={{ bgcolor: row.patronTypeColor || undefined }} />
                                    : <Typography></Typography>}
                            </Box>
                        </Box> */}
                    </Box>
                </Paper>

                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                    SIGNED DOCUMENTS
                </Typography>
                {row.documents.length === 0 && (
                    <Typography color="text.secondary" sx={{ ml: 1 }}>No documents found.</Typography>
                )}
                {row.documents.map((doc: SignedDocumentRow) => (
                    <Paper key={doc.patronSignatureId} variant="outlined"
                        sx={{ p: 1.5, mb: 1, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <ArticleIcon fontSize="small" color="action" />
                            <Box>
                                <Typography fontWeight={600} variant="body2">{doc.documentTypeName}</Typography>
                                <Typography variant="caption" color="text.secondary">
                                    {doc.fileName}  {fmtDateTime(doc.signedAt)}
                                </Typography>
                            </Box>
                        </Box>
                        <Stack direction="row" spacing={0.5} alignItems="center">
                            <Chip label={doc.status} size="small" color="success" variant="outlined" />
                            {doc.fileUrl && (
                                <Tooltip title="Download">
                                    <IconButton size="small" onClick={() =>
                                        handleDownloadDocument(doc)} color="success">
                                        <DownloadIcon fontSize="small" />
                                    </IconButton>
                                </Tooltip>
                            )}
                        </Stack>
                    </Paper>
                ))}
            </DialogContent>
            <DialogActions>
                <Button variant='outlined' onClick={onClose}>Close</Button>

                <Tooltip title="Send duplicate request to iPad via SignalR">
                    <Button startIcon={<FileCopyIcon />} variant="outlined" color="warning" onClick={() => onDuplicate(row)}>
                        Duplicate &amp; Re-sign
                    </Button>
                </Tooltip>
            </DialogActions>
        </Dialog>
    );
}

function DuplicateDialog({
    row, open, loading, onClose, onConfirm,
}: {
    row: SignedCustomerRow | null;
    open: boolean;
    loading: boolean;
    onClose: () => void;
    onConfirm: () => void;
}) {
    if (!row) return null;
    return (
        <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
            <DialogTitle>Duplicate &amp; Re-sign</DialogTitle>
            <DialogContent>
                <Typography>
                    Send a SignalR notification to the iPad to reload the previous session for{' '}
                    <strong>{row.customerName}</strong> ({row.displayId}) and ask them to re-sign?
                </Typography>
            </DialogContent>
            <DialogActions>
                <Button variant='outlined' onClick={onClose} disabled={loading}>Cancel</Button>
                <Button variant="contained" color="warning" onClick={onConfirm} disabled={loading}>
                    {loading ? <CircularProgress size={18} /> : 'Send Request'}
                </Button>
            </DialogActions>
        </Dialog>
    );
}

function CustomerRow({
    row,
    onView,
    onDuplicate
}: {
    row: SignedCustomerRow;
    onView: (r: SignedCustomerRow) => void;
    onDuplicate: (r: SignedCustomerRow) => void;
}) {
    const [expanded, setExpanded] = useState(false);
    const { showSnackbar } = useSnackbar();
    // Preview Dialog States
    const [previewDialogOpen, setPreviewDialogOpen] = useState(false);
    const [previewUrl, setPreviewUrl] = useState<string>('');
    const [previewTitle, setPreviewTitle] = useState<string>('');
    // Handle download
    const handleViewDownload = (url: string, filename: string) => {
        if (!url) {
            showSnackbar('Document URL is not available', 'error');
            return;
        }
        try {
            const link = document.createElement('a');
            link.href = url.startsWith('http') ? url : `${getApiBase()}${url}`;
            link.setAttribute('download', filename);
            link.setAttribute('target', '_blank');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (err: unknown) {
            const errorMessage = extractErrorMessage(err, "Failed to download document. Please try again.");
            logError('AdminCustomerSignedPage.handleViewDownload', err);
            showSnackbar(errorMessage, "error");
        }
    };

    const handleDownloadDocument = async (doc: SignedDocumentRow) => {
        if (!doc.fileUrl) {
            showSnackbar('Document URL is not available', 'error');
            return;
        }

        // Downlaod file when clicking the download button
        const link = document.createElement('a');
        const response = await fetch(doc.fileUrl.startsWith('http') ? doc.fileUrl : `${getApiBase()}${doc.fileUrl}`);
        const blob = await response.blob();
        link.href = URL.createObjectURL(blob);
        link.setAttribute('download', doc.fileName);
        link.setAttribute('target', '_blank');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        showSnackbar(`Downloading ${doc.fileName}`, 'success');
    }

    // Handle preview
    const handlePreview = (document: SignedDocumentRow) => {
        if (!document.fileUrl) return;
        const finalUrl = getApiBase() + document.fileUrl;
        setPreviewUrl(finalUrl);
        setPreviewTitle(document.fileName);
        setPreviewDialogOpen(true);
    };

    const handleClosePreview = () => {
        setPreviewDialogOpen(false);
        setPreviewUrl('');
        setPreviewTitle('');
    };

    const getLanguageDisplay = (lang: string | undefined): string => {
        if (!lang) return '';
        switch (lang.toUpperCase()) {
            case 'EN': return 'English';
            case 'VI': return 'Vietnamese';
            case 'KO': return 'Korean';
            case 'ZH': return 'Chinese';
            default: return lang;
        }
    }

    // Get file type from URL
    const getFileType = (url?: string): string => {
        if (!url) return 'unknown';
        const extension = url.split('.').pop()?.toLowerCase();
        if (extension === 'pdf') return 'PDF';
        if (['xls', 'xlsx'].includes(extension || '')) return 'Excel';
        if (['html', 'htm'].includes(extension || '')) return 'HTML';
        if (['jpg', 'jpeg', 'png', 'gif'].includes(extension || '')) return 'Image';
        if (['doc', 'docx'].includes(extension || '')) return 'Word';
        return 'Document';
    };

    // Handle print
    const handlePrint = (document: SignedDocumentRow) => {
        if (!document.fileUrl) {
            showSnackbar('Document URL is not available', 'error');
            return;
        }
        const finalUrl = getApiBase() + document.fileUrl;
        const printWindow = window.open(finalUrl, '_blank');
        if (printWindow) {
            printWindow.focus();
            printWindow.print();
        }
    };

    return (
        <>
            <TableRow hover>
                <TableCell>
                    <IconButton size="small" onClick={() => setExpanded(p => !p)}>
                        {expanded ? <ExpandLessIcon fontSize="small" /> : <ExpandMoreIcon fontSize="small" />}
                    </IconButton>
                </TableCell>
                <TableCell>
                    <Typography variant="body2" fontWeight={700} color={GREEN_TEAL}>{row.displayId}</Typography>
                </TableCell>
                <TableCell>
                    <Typography variant="body2" fontWeight={600}>{row.customerName}</Typography>
                    {row.email &&
                        <Typography variant="caption" color="text.secondary" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                            <EmailIcon fontSize="small" sx={{ mb: 0.5, color: 'primary.main' }} />
                            {row.email}
                        </Typography>
                    }
                    {row.phoneNumber &&
                        <>
                            <Typography variant="caption" color="text.secondary" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                                <PhoneIcon fontSize="small" sx={{ mb: 0.5, color: 'primary.main' }} />
                                {row.phoneNumber}
                            </Typography>
                        </>
                    }
                </TableCell>
                <TableCell>{row.nationality || ''}</TableCell>
                <TableCell>
                    {row.patronType
                        ? <Chip label={row.patronType} size="small"
                            sx={{ bgcolor: row.patronTypeColor || undefined, color: '#fff' }} />
                        : ''}

                    {row.customerType
                        ? <Chip label={row.customerType} size="small" color={CUSTOMER_TYPE_COLORS[row.customerType] ?? 'default'} sx={{ ml: 0.5 }} />
                        : ''}
                </TableCell>
                <TableCell>{row.roomNumber || ''}</TableCell>
                <TableCell>{row.outletName || ''}</TableCell>
                {/* <TableCell>
                    {row.customerType
                        ? <Chip label={row.customerType} size="small" color={CUSTOMER_TYPE_COLORS[row.customerType] ?? 'default'} />
                        : ''}
                </TableCell> */}
                <TableCell>{getLanguageDisplay(row.language)}</TableCell>
                <TableCell>{FormatUtcTime.formatDateDDMMMYYYYV2(row.signedAt)}</TableCell>
                {/* <TableCell>{row.signedBy || ''}</TableCell> */}
                <TableCell>{row.signedByDevice || ''}</TableCell>
                {/* <TableCell>
                    <Chip label={row.documents.length} size="small"
                        color={row.documents.length > 0 ? 'primary' : 'default'} />
                </TableCell> */}
                <TableCell>
                    <Stack direction="row" spacing={0.5}>
                        <Tooltip title="View detail">
                            <IconButton size="small" color='primary' onClick={() => onView(row)}>
                                <VisibilityIcon fontSize="small" />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title="Duplicate & re-sign on iPad">
                            <IconButton size="small" color="warning" onClick={() => onDuplicate(row)}>
                                <FileCopyIcon fontSize="small" />
                            </IconButton>
                        </Tooltip>
                    </Stack>
                </TableCell>
            </TableRow>

            {/* Expandable documents sub-row */}
            <TableRow>
                <TableCell colSpan={10} sx={{ py: 0, border: 0 }}>
                    <Collapse in={expanded} timeout="auto" unmountOnExit>
                        <Box sx={{ p: 1.5, bgcolor: 'grey.50', borderBottom: '1px solid', borderColor: 'divider' }}>
                            <Typography variant="caption" color="text.secondary" fontWeight={700} sx={{ ml: 1 }}>
                                SIGNED DOCUMENT{row.documents.length !== 1 ? 'S' : ''} ({row.documents.length})
                            </Typography>
                            {row.documents.length === 0 && (
                                <Typography variant="caption" color="text.secondary" sx={{ ml: 2 }}>No documents.</Typography>
                            )}
                            <Stack spacing={0.5} sx={{ mt: 0.5, m: 2 }}>
                                <Paper variant="outlined" sx={{ p: 2 }}>
                                    <List>
                                        {row.documents.map((document, index) => (
                                            <React.Fragment key={document.patronSignatureId}>
                                                <ListItem
                                                    disablePadding
                                                    secondaryAction={
                                                        <>
                                                            <Tooltip title="Preview">
                                                                <IconButton
                                                                    color="primary"
                                                                    size="small"
                                                                    onClick={() => handlePreview(document)}
                                                                >
                                                                    <VisibilityIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>

                                                            <Tooltip title="Print">
                                                                <IconButton
                                                                    color="primary"
                                                                    size="small"
                                                                    onClick={() => handlePrint(document)}
                                                                >
                                                                    <PrintIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>

                                                            <Tooltip title="Download">
                                                                <IconButton
                                                                    color="success"
                                                                    size="small"
                                                                    onClick={() => handleDownloadDocument(document)}
                                                                >
                                                                    <DownloadIcon fontSize="small" />
                                                                </IconButton>
                                                            </Tooltip>
                                                        </>
                                                    }
                                                >
                                                    <ListItemButton
                                                        onClick={() => handlePreview(document)}
                                                        sx={{ pr: 12 }}
                                                    >
                                                        <ListItemIcon>
                                                            <PDFIcon color="primary" />
                                                        </ListItemIcon>
                                                        <ListItemText
                                                            primary={
                                                                <Typography variant="body1" fontWeight={500}>
                                                                    {document.fileName}
                                                                </Typography>
                                                            }
                                                            secondary={
                                                                <Stack direction="row" spacing={2} sx={{ mt: 0.5 }}>
                                                                    {/* <Chip
                                                                        label={getFileType(document.fileUrl) || 'Document'}
                                                                        size="small"
                                                                        variant="outlined"
                                                                    /> */}
                                                                    <Typography variant="caption" color="text.secondary">
                                                                        Signed: {FormatUtcTime.formatDateDDMMMYYYY(document.signedAt)}
                                                                    </Typography>
                                                                </Stack>
                                                            }
                                                        />
                                                    </ListItemButton>
                                                </ListItem>
                                                {index < row.documents.length - 1 && <Divider />}
                                            </React.Fragment>
                                        ))}
                                    </List>
                                </Paper>
                            </Stack>
                        </Box>
                    </Collapse>
                </TableCell>
            </TableRow>

            {/* Preview Dialog */}
            <Dialog
                open={previewDialogOpen}
                onClose={handleClosePreview}
                maxWidth="lg"
                fullWidth
                PaperProps={{
                    sx: { height: '90vh' }
                }}
            >
                <DialogTitle>
                    <Box display="flex" justifyContent="space-between" alignItems="center">
                        <Typography variant="h6">{previewTitle}</Typography>
                        <IconButton onClick={handleClosePreview} size="small">
                            <CloseIcon />
                        </IconButton>
                    </Box>
                </DialogTitle>
                <DialogContent dividers sx={{ p: 0, height: '100%' }}>
                    {(() => {
                        const fileType = getFileType(previewUrl);
                        if (fileType === 'Image') {
                            return (
                                <Box
                                    sx={{
                                        display: 'flex',
                                        justifyContent: 'center',
                                        alignItems: 'center',
                                        height: '100%',
                                        bgcolor: 'grey.100'
                                    }}
                                >
                                    <img
                                        src={previewUrl}
                                        alt="Document"
                                        style={{
                                            maxWidth: '100%',
                                            maxHeight: '100%',
                                            objectFit: 'contain'
                                        }}
                                    />
                                </Box>
                            );
                        } else if (fileType === 'PDF') {
                            return (
                                <iframe
                                    src={previewUrl}
                                    title="PDF Viewer"
                                    style={{
                                        width: '100%',
                                        height: '100%',
                                        border: 'none'
                                    }}
                                />
                            );
                        } else if (fileType === 'Word' || fileType === 'Excel') {
                            return (
                                <Box textAlign="center" p={4}>
                                    <Typography variant="h6" color="text.secondary" gutterBottom>
                                        Preview not available
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary" paragraph>
                                        This file type ({fileType}) cannot be previewed in the browser. <br />
                                        You can download the file to view it.
                                    </Typography>
                                </Box>
                            );
                        } else {
                            return (
                                <iframe
                                    src={previewUrl}
                                    title="Document Viewer"
                                    style={{
                                        width: '100%',
                                        height: '100%',
                                        border: 'none'
                                    }}
                                />
                            );
                        }
                    })()}
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClosePreview} variant="outlined">
                        Close
                    </Button>
                    <Button
                        variant="contained"
                        startIcon={<DownloadIcon />}
                        onClick={() => {
                            handleViewDownload(previewUrl, previewTitle);
                        }}
                    >
                        Download
                    </Button>
                </DialogActions>
            </Dialog>

        </>
    );
}

export default function AdminCustomerSignedPage() {
    useSetPageTitle(PAGE_TITLES.CUSTOMER_SIGNED_DOCUMENTS);
    const muiTheme = useTheme();
    const { showSnackbar } = useSnackbar();
    const { outlets } = useAppData();
    const [searchTerm, setSearchTerm] = useState('');
    const [fromDate, setFromDate] = useState('');
    const [toDate, setToDate] = useState('');
    const [customerType, setCustomerType] = useState('');
    const [outletId, setOutletId] = useState<number | null>(null);
    const [pageSize, setPageSize] = useState(25);
    const [page, setPage] = useState(1);

    const [loading, setLoading] = useState(false);
    const [rows, setRows] = useState<SignedCustomerRow[]>([]);
    const [total, setTotal] = useState(0);

    const [detailRow, setDetailRow] = useState<SignedCustomerRow | null>(null);
    const [detailOpen, setDetailOpen] = useState(false);
    const [dupRow, setDupRow] = useState<SignedCustomerRow | null>(null);
    const [dupOpen, setDupOpen] = useState(false);
    const [dupLoading, setDupLoading] = useState(false);

    const searchTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

    const load = useCallback(async (p: number) => {
        setLoading(true);
        try {
            const result = await customerSignedService.getSignedCustomersAsync({
                page: p,
                pageSize,
                searchTerm: searchTerm || undefined,
                fromDate: fromDate || undefined,
                toDate: toDate || undefined,
                customerType: customerType || undefined,
                outletId: outletId || undefined,
            });
            setRows(result.data);
            setTotal(result.totalRecords);
        } catch (err) {
            console.error('[AdminCustomerSignedPage] load error', err);
        } finally {
            setLoading(false);
        }
    }, [searchTerm, fromDate, toDate, customerType, outletId, pageSize]);

    useEffect(() => {
        if (searchTimer.current) clearTimeout(searchTimer.current);
        searchTimer.current = setTimeout(() => { setPage(1); load(1); }, 350);
        return () => { if (searchTimer.current) clearTimeout(searchTimer.current); };
    }, [load]);

    const handlePageChange = (_: React.ChangeEvent<unknown>, value: number) => {
        setPage(value);
        load(value);
    };

    const handleDuplicate = async () => {
        if (!dupRow) return;
        setDupLoading(true);
        try {
            await api.post('/api/customer-sign/request', { patronId: dupRow.id });
        } catch (err) {
            console.error('[AdminCustomerSignedPage] duplicate error', err);
            showSnackbar('Failed to send duplicate request. Please try again.', 'error');
        } finally {
            setDupLoading(false);
            setDupOpen(false);
        }
    };

    // Handle download
    const handleViewDownload = (url: string, filename: string) => {
        if (!url) {
            showSnackbar('Document URL is not available', 'error');
            return;
        }
        try {
            const link = document.createElement('a');
            link.href = url.startsWith('http') ? url : `${getApiBase()}${url}`;
            link.setAttribute('download', filename);
            link.setAttribute('target', '_blank');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (err: unknown) {
            const errorMessage = extractErrorMessage(err, "Failed to download document. Please try again.");
            logError('AdminCustomerSignedPage.handleViewDownload', err);
            showSnackbar(errorMessage, "error");
        }
    };

    const totalPages = Math.max(1, Math.ceil(total / pageSize));

    return (
        <AdminLayout>
            <Box sx={{ p: 3 }}>

                {/* Header */}
                <Stack direction="row" justifyContent="space-between" alignItems="flex-start" mb={2.5}>
                    <Box>
                        {/* <Typography variant="h5" fontWeight={700} color={GREEN_TEAL}>
                            Signed Customers
                        </Typography> */}
                        <Typography variant="body2" color="text.secondary">
                            Guests who have completed and signed their outlets documents
                        </Typography>
                    </Box>
                    <Tooltip title="Refresh">
                        <IconButton onClick={() => load(page)} disabled={loading}>
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                </Stack>

                {/* Filters */}
                <Card variant="outlined" sx={{ mb: 2 }}>
                    <CardContent sx={{ pb: '12px !important' }}>
                        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} flexWrap="wrap" useFlexGap>
                            <TextField
                                size="small"
                                placeholder="Search name or room or phone..."
                                value={searchTerm}
                                onChange={e => setSearchTerm(e.target.value)}
                                sx={{ minWidth: 300 }}
                                InputProps={{
                                    startAdornment: (
                                        <InputAdornment position="start">
                                            <SearchIcon fontSize="small" />
                                        </InputAdornment>
                                    ),
                                }}
                            />
                            <TextField
                                size="small" type="date" label="From date"
                                value={fromDate}
                                onChange={e => setFromDate(e.target.value)}
                                InputLabelProps={{ shrink: true }}
                                sx={{ width: 160 }}
                            />
                            <TextField
                                size="small" type="date" label="To date"
                                value={toDate}
                                onChange={e => setToDate(e.target.value)}
                                InputLabelProps={{ shrink: true }}
                                sx={{ width: 160 }}
                            />

                            <FormControl size="small" sx={{ width: 140 }}>
                                <InputLabel>Type</InputLabel>
                                <Select value={customerType} label="Type" onChange={e => setCustomerType(e.target.value)}>
                                    <MenuItem value="">All</MenuItem>
                                    <MenuItem value="InHouse">In-House</MenuItem>
                                    <MenuItem value="WalkIn">Walk-In</MenuItem>
                                </Select>
                            </FormControl>

                            <Autocomplete
                                options={outlets}
                                size="small"
                                getOptionLabel={(option: OutletResponse) => option.name}
                                value={outletId ? outlets.find(o => o.id === outletId) || null : null}
                                onChange={(_, newValue) => setOutletId(newValue ? newValue.id : null)}
                                isOptionEqualToValue={(option, value) => option.id === value.id}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Outlet"
                                        placeholder="Select outlet"
                                    />
                                )}

                                sx={{ width: 200 }}
                            />

                            <FormControl size="small" sx={{ width: 110 }}>
                                <InputLabel>Per page</InputLabel>
                                <Select
                                    value={pageSize} label="Per page"
                                    onChange={e => { setPageSize(Number(e.target.value)); setPage(1); }}
                                >
                                    {PAGE_SIZE_OPTIONS.map(n => <MenuItem key={n} value={n}>{n}</MenuItem>)}
                                </Select>
                            </FormControl>
                        </Stack>
                    </CardContent>
                </Card>

                {/* Stats row */}
                <Stack direction="row" justifyContent="space-between" alignItems="center" mb={1}>
                    <Typography variant="body2" color="text.secondary">
                        {loading ? 'Loading' : `${total.toLocaleString()} record${total !== 1 ? 's' : ''} found`}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Page {page} / {totalPages}
                    </Typography>
                </Stack>

                {/* Table */}
                <Card variant="outlined">
                    <CardContent sx={{ p: 0, '&:last-child': { pb: 0 } }}>
                        <TableContainer>
                            <Table>
                                <TableHead>
                                    <TableRow sx={{ bgcolor: `${muiTheme.palette.primary.main}`, "& .MuiTableCell-root": { color: "#fff", fontWeight: 600 } }}>
                                        <TableCell width={30} />
                                        <TableCell width={80}>ID</TableCell>
                                        <TableCell>Customer Name</TableCell>
                                        {/* <TableCell>Phone Number</TableCell> */}
                                        <TableCell width={120}>Nationality</TableCell>
                                        <TableCell>Type</TableCell>
                                        <TableCell width={65}>Room/Ref</TableCell>
                                        <TableCell>Outlet</TableCell>
                                        <TableCell>Language</TableCell>
                                        <TableCell width={120}>Date & Time</TableCell>
                                        {/* <TableCell>Signed By</TableCell> */}
                                        <TableCell>Device</TableCell>
                                        <TableCell>Actions</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {loading && (
                                        <TableRow>
                                            <TableCell colSpan={11} align="center" sx={{ py: 6 }}>
                                                <CircularProgress size={36} />
                                            </TableCell>
                                        </TableRow>
                                    )}
                                    {!loading && rows.length === 0 && (
                                        <TableRow>
                                            <TableCell colSpan={11} align="center" sx={{ py: 6, color: 'text.secondary' }}>
                                                No records found
                                            </TableCell>
                                        </TableRow>
                                    )}
                                    {!loading && rows.map(row => (
                                        <CustomerRow
                                            key={row.id}
                                            row={row}
                                            onView={r => { setDetailRow(r); setDetailOpen(true); }}
                                            onDuplicate={r => { setDupRow(r); setDupOpen(true); }}
                                        />
                                    ))}
                                </TableBody>
                            </Table>
                        </TableContainer>

                        {/* Pagination */}
                        {!loading && totalPages > 1 && (
                            <>
                                <Divider />
                                <Box sx={{ display: 'flex', justifyContent: 'center', py: 1.5 }}>
                                    <Pagination
                                        count={totalPages}
                                        page={page}
                                        onChange={handlePageChange}
                                        color="primary"
                                        size="small"
                                    />
                                </Box>
                            </>
                        )}
                    </CardContent>
                </Card>
            </Box>

            <DetailDialog
                row={detailRow}
                open={detailOpen}
                onClose={() => setDetailOpen(false)}
                onDuplicate={r => { setDetailOpen(false); setDupRow(r); setDupOpen(true); }}
            />

            <DuplicateDialog
                row={dupRow}
                open={dupOpen}
                loading={dupLoading}
                onClose={() => setDupOpen(false)}
                onConfirm={handleDuplicate}
            />
        </AdminLayout>
    );
}

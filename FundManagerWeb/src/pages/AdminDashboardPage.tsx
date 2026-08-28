import React, { useEffect, useState, useCallback } from 'react';
import {
    Box,
    Card,
    CardContent,
    Typography,
    Grid,
    Stack,
    Chip,
    CircularProgress,
    Alert,
    Divider,
    Avatar,
    LinearProgress,
    Tooltip,
    IconButton,
    useTheme,
    alpha,
    Paper,
} from '@mui/material';
import {
    PeopleAlt as PeopleIcon,
    Assignment as AssignmentIcon,
    CardMembership as MembershipIcon,
    VerifiedUser as VerifiedIcon,
    Language as OnlineIcon,
    EditNote as ManualIcon,
    CheckCircle as CheckIcon,
    SmsFailedOutlined as SmsIcon,
    Flag as FlagIcon,
    PublicOutlined as ForeignerIcon,
    Refresh as RefreshIcon,
    FiberManualRecord as DotIcon,
    TrendingUp as TrendingUpIcon,
    TrendingDown as TrendingDownIcon,
} from '@mui/icons-material';
import AdminLayout from '../components/layout/AdminLayout';
import { useSetPageTitle } from '../hooks/useSetPageTitle';
import { PAGE_TITLES } from '../constants/pageTitles';
import { dashboardService } from '../services/dashboradService';
import type { DashboardSummaryResponse, FormSignedCount, LatestActivity } from '../types/dashboardType';
import WeeklyChartComponent from '../components/WeeklyChart';
import { FormatUtcTime } from '../utils/formatUtcTime';

// ─── Stat Card ───────────────────────────────────────────────────────────────

interface StatCardProps {
    title: string;
    value: number | string;
    icon: React.ReactNode;
    color: string;
    subtitle?: string;
    trend?: number;
}

const StatCard: React.FC<StatCardProps> = ({ title, value, icon, color, subtitle, trend }) => {
    const theme = useTheme();
    return (
        <Card
            elevation={0}
            sx={{
                border: `1px solid ${theme.palette.divider}`,
                borderRadius: 3,
                height: '100%',
                position: 'relative',
                overflow: 'hidden',
                transition: 'box-shadow 0.2s, transform 0.2s',
                '&:hover': {
                    boxShadow: theme.shadows[6],
                    transform: 'translateY(-2px)',
                },
            }}
        >
            <Box
                sx={{
                    position: 'absolute',
                    top: -20,
                    right: -20,
                    width: 100,
                    height: 100,
                    borderRadius: '50%',
                    bgcolor: alpha(color, 0.08),
                }}
            />
            <CardContent sx={{ p: 2.5, '&:last-child': { pb: 2.5 } }}>
                <Stack direction="row" justifyContent="space-between" alignItems="flex-start">
                    <Box flex={1}>
                        <Typography variant="caption" color="text.secondary" fontWeight={500} sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
                            {title}
                        </Typography>
                        <Typography variant="h4" fontWeight={800} sx={{ color, mt: 0.5, lineHeight: 1.2 }}>
                            {typeof value === 'number' ? value.toLocaleString() : value}
                        </Typography>
                        {subtitle && (
                            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
                                {subtitle}
                            </Typography>
                        )}
                        {trend !== undefined && (
                            <Stack direction="row" alignItems="center" gap={0.5} mt={0.5}>
                                {trend >= 0 ? (
                                    <TrendingUpIcon sx={{ fontSize: 14, color: 'success.main' }} />
                                ) : (
                                    <TrendingDownIcon sx={{ fontSize: 14, color: 'error.main' }} />
                                )}
                                <Typography variant="caption" color={trend >= 0 ? 'success.main' : 'error.main'} fontWeight={600}>
                                    {Math.abs(trend)}% vs last week
                                </Typography>
                            </Stack>
                        )}
                    </Box>
                    <Avatar sx={{ bgcolor: alpha(color, 0.12), color, width: 48, height: 48 }}>
                        {icon}
                    </Avatar>
                </Stack>
            </CardContent>
        </Card>
    );
};

// ─── Form Signed Progress ─────────────────────────────────────────────────────

const FORM_COLORS: Record<number, string> = {
    1: '#6366f1',
    2: '#22d3ee',
    3: '#f59e0b',
    4: '#10b981',
};

const FORM_LABELS: Record<number, string> = {
    1: 'HTR Form',
    2: 'PDP Notification',
    3: 'HTP Confirmation',
    4: 'HTR Membership T&C',
};

interface FormProgressProps {
    items: FormSignedCount[];
    total: number;
}

const FormSignedProgress: React.FC<FormProgressProps> = ({ items, total }) => {
    const theme = useTheme();
    return (
        <Card elevation={0} sx={{ border: `1px solid ${theme.palette.divider}`, borderRadius: 3, height: '100%' }}>
            <CardContent sx={{ p: 3, '&:last-child': { pb: 3 } }}>
                <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2.5}>
                    <Box>
                        <Typography variant="h6" fontWeight={700}>Documents Signed</Typography>
                        <Typography variant="body2" color="text.secondary">Patron signature completion by document type</Typography>
                    </Box>
                    <Chip
                        label={`${total.toLocaleString()} patrons`}
                        size="small"
                        sx={{ bgcolor: 'primary.50', color: 'primary.main', fontWeight: 700 }}
                    />
                </Stack>
                <Stack spacing={2.5}>
                    {items.map((item) => {
                        const pct = total > 0 ? Math.round((item.count / total) * 100) : 0;
                        const color = FORM_COLORS[item.documentTypeValue] ?? '#94a3b8';
                        const label = FORM_LABELS[item.documentTypeValue] ?? item.documentType;
                        return (
                            <Box key={item.documentTypeValue}>
                                <Stack direction="row" justifyContent="space-between" mb={0.75}>
                                    <Stack direction="row" alignItems="center" gap={1}>
                                        <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: color }} />
                                        <Typography variant="body2" fontWeight={600}>{label}</Typography>
                                    </Stack>
                                    <Stack direction="row" alignItems="center" gap={1}>
                                        <Typography variant="body2" fontWeight={700} sx={{ color }}>
                                            {item.count.toLocaleString()}
                                        </Typography>
                                        <Typography variant="caption" color="text.secondary">({pct}%)</Typography>
                                    </Stack>
                                </Stack>
                                <LinearProgress
                                    variant="determinate"
                                    value={pct}
                                    sx={{
                                        height: 8,
                                        borderRadius: 4,
                                        bgcolor: alpha(color, 0.12),
                                        '& .MuiLinearProgress-bar': { bgcolor: color, borderRadius: 4 },
                                    }}
                                />
                            </Box>
                        );
                    })}
                </Stack>
            </CardContent>
        </Card>
    );
};

// ─── Activity Feed ────────────────────────────────────────────────────────────

const ActivityFeed: React.FC<{ activities: LatestActivity[] }> = ({ activities }) => {
    const theme = useTheme();
    return (
        <Card elevation={0} sx={{ border: `1px solid ${theme.palette.divider}`, borderRadius: 3, height: '100%' }}>
            <CardContent sx={{ p: 3, '&:last-child': { pb: 3 } }}>
                <Typography variant="h6" fontWeight={700} mb={0.5}>Latest Activity</Typography>
                <Typography variant="body2" color="text.secondary" mb={2.5}>Recent system events</Typography>
                <Stack spacing={0} divider={<Divider />}>
                    {activities.slice(0, 8).map((a, idx) => (
                        <Box key={idx} py={1.5}>
                            <Stack direction="row" alignItems="flex-start" gap={1.5}>
                                <Box mt={0.5}>
                                    <DotIcon
                                        sx={{
                                            fontSize: 12,
                                            color: a.isSuccess ? 'success.main' : 'error.main',
                                        }}
                                    />
                                </Box>
                                <Box flex={1} minWidth={0}>
                                    <Stack direction="row" justifyContent="space-between" alignItems="flex-start" flexWrap="wrap" gap={0.5}>
                                        <Typography
                                            variant="body2"
                                            fontWeight={600}
                                            noWrap
                                            sx={{ maxWidth: 200 }}
                                            title={a.userName}
                                        >
                                            {a.userName.toUpperCase()}
                                        </Typography>
                                        <Typography variant="caption" color="text.disabled" sx={{ whiteSpace: 'nowrap' }}>
                                            {FormatUtcTime.formatDateTime(a.createdAt)}
                                        </Typography>
                                    </Stack>
                                    <Typography variant="caption" color="text.secondary" component="div">
                                        <b>{a.action}</b>
                                        {a.entityId ? ` #${a.entityId}` : ''}
                                    </Typography>
                                    {a.details && (
                                        <Typography
                                            variant="caption"
                                            color="text.disabled"
                                            sx={{ display: 'block', mt: 0.25, wordBreak: 'break-word', fontWeight: 'bold' }}
                                        >
                                            {a.details}
                                        </Typography>
                                    )}
                                </Box>
                                <Chip
                                    label={a.isSuccess ? 'OK' : 'Fail'}
                                    size="small"
                                    color={a.isSuccess ? 'success' : 'error'}
                                    sx={{ fontWeight: 700, fontSize: 10, height: 20, minWidth: 42, flexShrink: 0 }}
                                />
                            </Stack>
                        </Box>
                    ))}
                    {activities.length === 0 && (
                        <Typography variant="body2" color="text.disabled" textAlign="center" py={4}>
                            No recent activity
                        </Typography>
                    )}
                </Stack>
            </CardContent>
        </Card>
    );
};

// ─── Nationality Breakdown ───────────────────────────────────────────────────

interface NationalityCardProps {
    totalVietnamese: number;
    totalForeigners: number;
}

const NationalityCard: React.FC<NationalityCardProps> = ({ totalVietnamese, totalForeigners }) => {
    const theme = useTheme();
    const total = totalVietnamese + totalForeigners;
    const vnPct = total > 0 ? Math.round((totalVietnamese / total) * 100) : 0;
    const foPct = total > 0 ? 100 - vnPct : 0;

    return (
        <Card elevation={0} sx={{ border: `1px solid ${theme.palette.divider}`, borderRadius: 3, height: '100%' }}>
            <CardContent sx={{ p: 3, '&:last-child': { pb: 3 } }}>
                <Typography variant="h6" fontWeight={700} mb={0.5}>Patron Nationality</Typography>
                <Typography variant="body2" color="text.secondary" mb={3}>Vietnamese vs Foreigners</Typography>

                {/* Segmented bar */}
                <Box sx={{ height: 12, borderRadius: 6, overflow: 'hidden', display: 'flex', mb: 2.5 }}>
                    <Box sx={{ width: `${vnPct}%`, bgcolor: '#6366f1', transition: 'width 0.6s ease' }} />
                    <Box sx={{ flex: 1, bgcolor: '#f59e0b' }} />
                </Box>

                <Stack spacing={1.5}>
                    {[
                        { label: 'Vietnamese', value: totalVietnamese, pct: vnPct, color: '#6366f1', icon: <FlagIcon fontSize="small" /> },
                        { label: 'Foreigners', value: totalForeigners, pct: foPct, color: '#f59e0b', icon: <ForeignerIcon fontSize="small" /> },
                    ].map((item) => (
                        <Stack key={item.label} direction="row" alignItems="center" justifyContent="space-between">
                            <Stack direction="row" alignItems="center" gap={1}>
                                <Avatar sx={{ bgcolor: alpha(item.color, 0.12), color: item.color, width: 32, height: 32 }}>
                                    {item.icon}
                                </Avatar>
                                <Box>
                                    <Typography variant="body2" fontWeight={600}>{item.label}</Typography>
                                    <Typography variant="caption" color="text.secondary">{item.pct}% of total</Typography>
                                </Box>
                            </Stack>
                            <Typography variant="h6" fontWeight={800} sx={{ color: item.color }}>
                                {item.value.toLocaleString()}
                            </Typography>
                        </Stack>
                    ))}
                </Stack>
            </CardContent>
        </Card>
    );
};

// ─── Main Page ────────────────────────────────────────────────────────────────

const AdminDashboardPage: React.FC = () => {
    useSetPageTitle(PAGE_TITLES.DASHBOARD);
    const theme = useTheme();

    const [data, setData] = useState<DashboardSummaryResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string>('');
    const [lastRefresh, setLastRefresh] = useState<Date>(new Date());

    const fetchData = useCallback(async () => {
        setLoading(true);
        setError('');
        try {
            const result = await dashboardService.getDashboardSummary();
            setData(result);
            setLastRefresh(new Date());
        } catch (e: unknown) {
            setError(e instanceof Error ? e.message : 'Failed to load dashboard data.');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    const registrationPct = data && data.totalRegistrations > 0
        ? Math.round((data.totalOnlineRegistrations / data.totalRegistrations) * 100)
        : 0;

    return (
        <AdminLayout>
            <Box sx={{ p: { xs: 2, md: 3 }, maxWidth: 1600, mx: 'auto' }}>
                {/* ── Header ── */}
                <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3} flexWrap="wrap" gap={2}>
                    <Box>
                        {/* <Typography variant="h4" fontWeight={800} letterSpacing={-0.5}>
                            Operations Dashboard
                        </Typography> */}
                        <Typography variant="body2" color="text.secondary">
                            Live overview of the HTR portal system
                        </Typography>
                    </Box>
                    <Stack direction="row" alignItems="center" gap={1.5}>
                        <Typography variant="caption" color="text.disabled">
                            Last updated: {lastRefresh.toLocaleTimeString()}
                        </Typography>
                        <Tooltip title="Refresh">
                            <IconButton onClick={fetchData} disabled={loading} size="small" sx={{ border: `1px solid ${theme.palette.divider}` }}>
                                <RefreshIcon fontSize="small" sx={{ animation: loading ? 'spin 1s linear infinite' : 'none', '@keyframes spin': { '0%': { transform: 'rotate(0deg)' }, '100%': { transform: 'rotate(360deg)' } } }} />
                            </IconButton>
                        </Tooltip>
                    </Stack>
                </Stack>

                {error && (
                    <Alert severity="error" sx={{ mb: 3, borderRadius: 2 }}>
                        {error}
                    </Alert>
                )}

                {loading && !data && (
                    <Box display="flex" justifyContent="center" alignItems="center" minHeight={400}>
                        <Stack alignItems="center" gap={2}>
                            <CircularProgress size={48} />
                            <Typography variant="body2" color="text.secondary">Loading dashboard...</Typography>
                        </Stack>
                    </Box>
                )}

                {data && (
                    <Stack spacing={3}>
                        {/* ── Row 1: Key Stats ── */}
                        <Grid container spacing={2.5}>
                            {[
                                {
                                    title: 'Total',
                                    value: data.totalRegistrations,
                                    icon: <AssignmentIcon />,
                                    color: '#6366f1',
                                    subtitle: 'All-time registrations',
                                },
                                {
                                    title: 'Online',
                                    value: data.totalOnlineRegistrations,
                                    icon: <OnlineIcon />,
                                    color: '#22d3ee',
                                    subtitle: `${registrationPct}% of total`,
                                },
                                {
                                    title: 'Manual',
                                    value: data.totalManualRegistrations,
                                    icon: <ManualIcon />,
                                    color: '#f59e0b',
                                    subtitle: `${100 - registrationPct}% of total`,
                                },
                                {
                                    title: 'Total Memberships',
                                    value: data.totalMemberships,
                                    icon: <MembershipIcon />,
                                    color: '#10b981',
                                    subtitle: 'Active members',
                                },
                                {
                                    title: 'Forms Signed',
                                    value: data.totalFormSignedPatrons,
                                    icon: <VerifiedIcon />,
                                    color: '#8b5cf6',
                                    subtitle: 'Patrons signed docs',
                                },
                                {
                                    title: 'OTP Verified',
                                    value: data.totalOtpVerified,
                                    icon: <SmsIcon />,
                                    color: '#ec4899',
                                    subtitle: 'Phone verifications',
                                },
                            ].map((stat) => (
                                <Grid size={{ xs: 12, sm: 6, md: 4, lg: 2 }} key={stat.title}>
                                    <StatCard {...stat} />
                                </Grid>
                            ))}
                        </Grid>

                        {/* ── Row 2: Online vs Manual visual split ── */}
                        <Paper
                            elevation={0}
                            sx={{
                                border: `1px solid ${theme.palette.divider}`,
                                borderRadius: 3,
                                p: 3,
                                background: `linear-gradient(135deg, ${alpha('#6366f1', 0.04)} 0%, ${alpha('#22d3ee', 0.04)} 100%)`,
                            }}
                        >
                            <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2} flexWrap="wrap" gap={1}>
                                <Typography variant="subtitle1" fontWeight={700}>Registration Channel Split</Typography>
                                <Stack direction="row" gap={1}>
                                    <Chip label="Online" size="small" sx={{ bgcolor: alpha('#6366f1', 0.1), color: '#6366f1', fontWeight: 700 }} />
                                    <Chip label="Manual" size="small" sx={{ bgcolor: alpha('#f59e0b', 0.1), color: '#f59e0b', fontWeight: 700 }} />
                                </Stack>
                            </Stack>
                            <Box sx={{ height: 20, borderRadius: 10, overflow: 'hidden', display: 'flex', bgcolor: theme.palette.action.hover }}>
                                <Tooltip title={`Online: ${data.totalOnlineRegistrations.toLocaleString()} (${registrationPct}%)`}>
                                    <Box sx={{ width: `${registrationPct}%`, bgcolor: '#6366f1', transition: 'width 0.8s ease', cursor: 'default', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                                        {registrationPct > 8 && (
                                            <Typography variant="caption" color="white" fontWeight={700}>{registrationPct}%</Typography>
                                        )}
                                    </Box>
                                </Tooltip>
                                <Tooltip title={`Manual: ${data.totalManualRegistrations.toLocaleString()} (${100 - registrationPct}%)`}>
                                    <Box sx={{ flex: 1, bgcolor: '#f59e0b', display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'default' }}>
                                        {(100 - registrationPct) > 8 && (
                                            <Typography variant="caption" color="white" fontWeight={700}>{100 - registrationPct}%</Typography>
                                        )}
                                    </Box>
                                </Tooltip>
                            </Box>
                            <Stack direction="row" justifyContent="space-between" mt={1.5}>
                                <Stack direction="row" alignItems="center" gap={1}>
                                    <CheckIcon sx={{ fontSize: 14, color: '#6366f1' }} />
                                    <Typography variant="caption" color="text.secondary">
                                        Online: <b style={{ color: '#6366f1' }}>{data.totalOnlineRegistrations.toLocaleString()}</b>
                                    </Typography>
                                </Stack>
                                <Stack direction="row" alignItems="center" gap={1}>
                                    <CheckIcon sx={{ fontSize: 14, color: '#f59e0b' }} />
                                    <Typography variant="caption" color="text.secondary">
                                        Manual: <b style={{ color: '#f59e0b' }}>{data.totalManualRegistrations.toLocaleString()}</b>
                                    </Typography>
                                </Stack>
                            </Stack>
                        </Paper>

                        {/* ── Row 3: Weekly Chart + Nationality ── */}
                        <Grid container spacing={2.5}>
                            <Grid size={{ xs: 12, lg: 12 }}>
                                <WeeklyChartComponent data={data.weeklyChart} />
                            </Grid>

                        </Grid>

                        {/* ── Row 4: Form Signed + Activity ── */}
                        <Grid container spacing={2.5}>
                            <Grid size={{ xs: 12, sm: 6, lg: 8 }}>
                                <FormSignedProgress
                                    items={data.formSignedCounts}
                                    total={data.totalFormSignedPatrons}
                                />
                            </Grid>
                            <Grid size={{ xs: 12, sm: 6, lg: 4 }}>
                                <NationalityCard
                                    totalVietnamese={data.totalVietnamese}
                                    totalForeigners={data.totalForeigners}
                                />
                            </Grid>

                        </Grid>

                        {/* Row 5 latestActivities */}
                        <Grid container spacing={2.5}>
                            <Grid size={{ xs: 12, lg: 12 }}>
                                <ActivityFeed activities={data.latestActivities} />
                            </Grid>
                        </Grid>
                    </Stack>
                )}
            </Box>
        </AdminLayout>
    );
};

export default AdminDashboardPage;

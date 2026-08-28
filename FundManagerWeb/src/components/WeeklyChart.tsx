import React, { useState, useEffect } from 'react';
import {
    Box,
    Card,
    CardContent,
    Typography,
    ToggleButton,
    ToggleButtonGroup,
    Tooltip,
    useTheme,
    Stack,
    Chip,
} from '@mui/material';
import {
    BarChart,
    Bar,
    LineChart,
    Line,
    ComposedChart,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip as RechartsTooltip,
    Legend,
    ResponsiveContainer,
} from 'recharts';
import BarChartIcon from '@mui/icons-material/BarChart';
import ShowChartIcon from '@mui/icons-material/ShowChart';
import StackedBarChartIcon from '@mui/icons-material/StackedBarChart';
import type { WeeklyChart as WeeklyChartData } from '../types/dashboardType';

type ChartMode = 'bar' | 'line' | 'stack';

const CHART_MODE_STORAGE_KEY = 'dashboard_weekly_chart_mode';

interface WeeklyChartProps {
    data: WeeklyChartData;
}

const COLORS = {
    thisWeekOnline: '#6366f1',
    thisWeekManual: '#22d3ee',
    lastWeekOnline: '#a78bfa',
    lastWeekManual: '#67e8f9',
};

const WeeklyChartComponent: React.FC<WeeklyChartProps> = ({ data }) => {
    const theme = useTheme();

    const [mode, setMode] = useState<ChartMode>(() => {
        const saved = localStorage.getItem(CHART_MODE_STORAGE_KEY);
        if (saved === 'bar' || saved === 'line' || saved === 'stack') return saved;
        return 'bar';
    });

    useEffect(() => {
        localStorage.setItem(CHART_MODE_STORAGE_KEY, mode);
    }, [mode]);

    const handleModeChange = (_: React.MouseEvent<HTMLElement>, newMode: ChartMode | null) => {
        if (newMode) setMode(newMode);
    };

    const chartData = (data.labels || []).map((label, i) => ({
        label,
        'This Week Online': data.thisWeekOnline?.[i] ?? 0,
        'This Week Manual': data.thisWeekManual?.[i] ?? 0,
        'Last Week Online': data.lastWeekOnline?.[i] ?? 0,
        'Last Week Manual': data.lastWeekManual?.[i] ?? 0,
    }));

    const commonProps = {
        data: chartData,
        margin: { top: 8, right: 16, left: 0, bottom: 0 },
    };

    const tooltipStyle = {
        backgroundColor: theme.palette.background.paper,
        border: `1px solid ${theme.palette.divider}`,
        borderRadius: 8,
        boxShadow: theme.shadows[4],
    };

    const renderBars = (stacked = false) => (
        <>
            <Bar dataKey="This Week Online" fill={COLORS.thisWeekOnline} radius={stacked ? [0, 0, 0, 0] : [4, 4, 0, 0]} stackId={stacked ? 'a' : undefined} />
            <Bar dataKey="This Week Manual" fill={COLORS.thisWeekManual} radius={stacked ? [4, 4, 0, 0] : [4, 4, 0, 0]} stackId={stacked ? 'a' : undefined} />
            <Bar dataKey="Last Week Online" fill={COLORS.lastWeekOnline} radius={stacked ? [0, 0, 0, 0] : [4, 4, 0, 0]} stackId={stacked ? 'b' : undefined} />
            <Bar dataKey="Last Week Manual" fill={COLORS.lastWeekManual} radius={stacked ? [4, 4, 0, 0] : [4, 4, 0, 0]} stackId={stacked ? 'b' : undefined} />
        </>
    );

    const renderLines = () => (
        <>
            <Line type="monotone" dataKey="This Week Online" stroke={COLORS.thisWeekOnline} strokeWidth={2.5} dot={{ r: 4 }} activeDot={{ r: 6 }} />
            <Line type="monotone" dataKey="This Week Manual" stroke={COLORS.thisWeekManual} strokeWidth={2.5} dot={{ r: 4 }} activeDot={{ r: 6 }} />
            <Line type="monotone" dataKey="Last Week Online" stroke={COLORS.lastWeekOnline} strokeWidth={2} strokeDasharray="5 5" dot={{ r: 3 }} activeDot={{ r: 5 }} />
            <Line type="monotone" dataKey="Last Week Manual" stroke={COLORS.lastWeekManual} strokeWidth={2} strokeDasharray="5 5" dot={{ r: 3 }} activeDot={{ r: 5 }} />
        </>
    );

    const gridProps = {
        stroke: theme.palette.divider,
        strokeDasharray: '3 3',
    };

    const xAxisProps = {
        dataKey: 'label' as const,
        tick: { fontSize: 12, fill: theme.palette.text.secondary },
        axisLine: false,
        tickLine: false,
    };

    const yAxisProps = {
        tick: { fontSize: 12, fill: theme.palette.text.secondary },
        axisLine: false,
        tickLine: false,
        width: 36,
    };

    return (
        <Card elevation={0} sx={{ border: `1px solid ${theme.palette.divider}`, borderRadius: 3, height: '100%' }}>
            <CardContent sx={{ p: 3, '&:last-child': { pb: 3 } }}>
                <Stack direction="row" justifyContent="space-between" alignItems="flex-start" mb={3} flexWrap="wrap" gap={1}>
                    <Box>
                        <Typography variant="h6" fontWeight={700}>Weekly Registrations</Typography>
                        <Typography variant="body2" color="text.secondary">This week vs last week comparison</Typography>
                    </Box>
                    <Stack direction="row" alignItems="center" gap={1}>
                        <ToggleButtonGroup
                            value={mode}
                            exclusive
                            onChange={handleModeChange}
                            size="small"
                            sx={{
                                '& .MuiToggleButton-root': {
                                    px: 1.5,
                                    py: 0.5,
                                    border: `1px solid ${theme.palette.divider}`,
                                    '&.Mui-selected': {
                                        bgcolor: 'primary.main',
                                        color: 'primary.contrastText',
                                        '&:hover': { bgcolor: 'primary.dark' },
                                    },
                                },
                            }}
                        >
                            <Tooltip title="Bar Chart">
                                <ToggleButton value="bar">
                                    <BarChartIcon fontSize="small" />
                                </ToggleButton>
                            </Tooltip>
                            <Tooltip title="Line Chart">
                                <ToggleButton value="line">
                                    <ShowChartIcon fontSize="small" />
                                </ToggleButton>
                            </Tooltip>
                            <Tooltip title="Stacked Bar">
                                <ToggleButton value="stack">
                                    <StackedBarChartIcon fontSize="small" />
                                </ToggleButton>
                            </Tooltip>
                        </ToggleButtonGroup>
                    </Stack>
                </Stack>

                {/* Legend */}
                <Stack direction="row" flexWrap="wrap" gap={1} mb={2}>
                    {Object.entries(COLORS).map(([key, color]) => (
                        <Chip
                            key={key}
                            size="small"
                            label={key.replace(/([A-Z])/g, ' $1').replace('this ', 'This ').replace('last ', 'Last ').trim()}
                            sx={{ bgcolor: color + '22', color: color, fontWeight: 600, fontSize: 11, border: `1px solid ${color}44` }}
                        />
                    ))}
                </Stack>

                <Box sx={{ width: '100%', height: 280 }}>
                    <ResponsiveContainer width="100%" height="100%">
                        {mode === 'line' ? (
                            <LineChart {...commonProps}>
                                <CartesianGrid {...gridProps} />
                                <XAxis {...xAxisProps} />
                                <YAxis {...yAxisProps} />
                                <RechartsTooltip contentStyle={tooltipStyle} />
                                <Legend wrapperStyle={{ display: 'none' }} />
                                {renderLines()}
                            </LineChart>
                        ) : mode === 'stack' ? (
                            <BarChart {...commonProps}>
                                <CartesianGrid {...gridProps} />
                                <XAxis {...xAxisProps} />
                                <YAxis {...yAxisProps} />
                                <RechartsTooltip contentStyle={tooltipStyle} />
                                <Legend wrapperStyle={{ display: 'none' }} />
                                {renderBars(true)}
                            </BarChart>
                        ) : (
                            <BarChart {...commonProps}>
                                <CartesianGrid {...gridProps} />
                                <XAxis {...xAxisProps} />
                                <YAxis {...yAxisProps} />
                                <RechartsTooltip contentStyle={tooltipStyle} />
                                <Legend wrapperStyle={{ display: 'none' }} />
                                {renderBars(false)}
                            </BarChart>
                        )}
                    </ResponsiveContainer>
                </Box>
            </CardContent>
        </Card>
    );
};

export default WeeklyChartComponent;

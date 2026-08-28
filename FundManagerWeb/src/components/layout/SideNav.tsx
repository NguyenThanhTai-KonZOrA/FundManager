import * as React from 'react';
import {
    Box,
    Stack,
    List,
    Typography,
    Divider,
} from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { useSidebar } from '../../contexts/SidebarContext';
import { usePermission } from '../../hooks/usePermission';
import { navItems } from './navConfig';
import { NavItemRenderer } from './NavItemRenderer';
import { filterNavItems, loadNavGroupsState, saveNavGroupsState } from '../../utils/navUtils';

export function SideNav(): React.JSX.Element {
    const { isCollapsed } = useSidebar();
    const { can } = usePermission();
    const muiTheme = useTheme();
    const sideNavGradient = `linear-gradient(165deg, ${muiTheme.palette.primary.dark} 0%, ${muiTheme.palette.primary.main} 38%, ${muiTheme.palette.primary.light} 78%, ${muiTheme.palette.secondary.main} 100%)`;

    // Load initial state from localStorage
    const [openGroups, setOpenGroups] = React.useState<Record<string, boolean>>(() =>
        loadNavGroupsState('sideNavOpenGroups')
    );

    const toggleGroup = (key: string) => {
        setOpenGroups(prev => {
            const isOpening = !prev[key];
            // Close all other groups when opening a new one
            const newState: Record<string, boolean> = {};
            for (const k of Object.keys(prev)) {
                newState[k] = false;
            }
            newState[key] = isOpening;
            saveNavGroupsState('sideNavOpenGroups', newState);
            return newState;
        });
    };

    const filteredNavItems = filterNavItems(navItems, can);

    return (
        <Box
            sx={{
                background: sideNavGradient,
                color: 'var(--mui-palette-common-white)',
                display: { xs: 'none', lg: 'flex' },
                flexDirection: 'column',
                height: '100vh',
                left: 0,
                maxWidth: '100%',
                position: 'fixed',
                top: 0,
                width: '290px',
                zIndex: 1100,
                transform: isCollapsed ? 'translateX(-100%)' : 'translateX(0)',
                transition: 'all 0.3s ease',
                boxShadow: '8px 0 30px rgba(8, 18, 20, 0.32)',
                borderRight: '1px solid rgba(220, 241, 244, 0.16)',
                overflow: 'hidden',
                '&::before': {
                    content: '""',
                    position: 'absolute',
                    top: -90,
                    right: -80,
                    width: 220,
                    height: 220,
                    borderRadius: '50%',
                    background: 'radial-gradient(circle, rgba(13, 147, 201, 0.28) 0%, rgba(13, 147, 201, 0) 72%)',
                    pointerEvents: 'none',
                },
                '&::after': {
                    content: '""',
                    position: 'absolute',
                    bottom: -120,
                    left: -80,
                    width: 260,
                    height: 260,
                    borderRadius: '50%',
                    background: 'radial-gradient(circle, rgba(201, 168, 76, 0.2) 0%, rgba(201, 168, 76, 0) 74%)',
                    pointerEvents: 'none',
                },
            }}
        >
            {/* Logo Section */}
            <Stack spacing={1} sx={{ p: 2, pb: 2, position: 'relative', zIndex: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', mb: 1 }}>
                    <img src="/images/TheGrandHoTram.png" alt="Logo" style={{ height: 130, width: 'auto' }} />
                </Box>
                <Box
                    sx={{
                        alignSelf: 'center',
                        px: 1.2,
                        py: 0.45,
                        borderRadius: 999,
                        fontSize: '0.66rem',
                        letterSpacing: 1,
                        fontWeight: 700,
                        color: '#d9f7ff',
                        border: '1px solid rgba(165, 228, 255, 0.42)',
                        bgcolor: 'rgba(6, 31, 35, 0.5)',
                    }}
                >
                    OPERATIONS CENTER
                </Box>
                <Typography
                    variant="h6"
                    sx={{
                        textAlign: 'center',
                        fontWeight: 800,
                        fontSize: '1.1rem',
                        letterSpacing: 0.7,
                        color: '#e8c96b',
                        textShadow: '0 3px 12px rgba(5, 15, 17, 0.35)',
                    }}
                >
                    Digital Document Platform
                </Typography>
            </Stack>

            <Divider sx={{ borderColor: 'rgba(228, 248, 251, 0.25)', mx: 2, position: 'relative', zIndex: 1 }} />

            {/* Navigation Items */}
            <Box
                sx={{
                    flex: '1 1 auto',
                    overflow: 'auto',
                    py: 2,
                    px: 0.4,
                    position: 'relative',
                    zIndex: 1,
                    '&::-webkit-scrollbar': {
                        width: '6px',
                    },
                    '&::-webkit-scrollbar-track': {
                        background: 'rgba(255, 255, 255, 0.08)',
                    },
                    '&::-webkit-scrollbar-thumb': {
                        background: 'linear-gradient(180deg, rgba(13, 147, 201, 0.65) 0%, rgba(92, 199, 216, 0.45) 100%)',
                        borderRadius: '3px',
                    },
                }}
            >
                <List
                    sx={{
                        px: 2,
                        py: 1.2,
                        borderRadius: 3,
                        background: 'linear-gradient(180deg, rgba(255, 255, 255, 0.1) 0%, rgba(255, 255, 255, 0.06) 100%)',
                        border: '1px solid rgba(255, 255, 255, 0.18)',
                        boxShadow: 'inset 0 1px 0 rgba(255, 255, 255, 0.08)',
                    }}
                >
                    {filteredNavItems.map((item) => (
                        <NavItemRenderer
                            key={item.key}
                            item={item}
                            openGroups={openGroups}
                            onToggleGroup={toggleGroup}
                        />
                    ))}
                </List>
            </Box>
        </Box>
    );
}
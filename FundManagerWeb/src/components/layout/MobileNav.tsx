import * as React from 'react';
import {
    Box,
    Drawer,
    Stack,
    List,
    Divider,
    Typography,
} from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { usePermission } from '../../hooks/usePermission';
import { navItems } from './navConfig';
import { NavItemRenderer } from './NavItemRenderer';
import { filterNavItems, loadNavGroupsState, saveNavGroupsState } from '../../utils/navUtils';

interface MobileNavProps {
    onClose: () => void;
    open: boolean;
}

export function MobileNav({ onClose, open }: MobileNavProps): React.JSX.Element {
    const { can } = usePermission();
    const muiTheme = useTheme();
    const mobileNavGradient = `linear-gradient(160deg, ${muiTheme.palette.primary.dark} 0%, ${muiTheme.palette.primary.main} 40%, ${muiTheme.palette.primary.light} 80%, ${muiTheme.palette.secondary.main} 100%)`;

    // Load initial state from localStorage
    const [openGroups, setOpenGroups] = React.useState<Record<string, boolean>>(() =>
        loadNavGroupsState('mobileNavOpenGroups')
    );

    const toggleGroup = (key: string) => {
        setOpenGroups(prev => {
            const newState = {
                ...prev,
                [key]: !prev[key]
            };
            saveNavGroupsState('mobileNavOpenGroups', newState);
            return newState;
        });
    };

    const filteredNavItems = filterNavItems(navItems, can);

    return (
        <Drawer
            anchor="left"
            onClose={onClose}
            open={open}
            PaperProps={{
                sx: {
                    background: mobileNavGradient,
                    color: 'var(--mui-palette-common-white)',
                    width: 'min(340px, 92vw)',
                    display: 'flex',
                    flexDirection: 'column',
                    borderRight: '1px solid rgba(220, 241, 244, 0.16)',
                    boxShadow: '10px 0 34px rgba(8, 18, 20, 0.34)',
                    overflow: 'hidden',
                    position: 'relative',
                    opacity: open ? 1 : 0,
                    animation: open ? 'mobileNavEnter 280ms cubic-bezier(0.2, 0.8, 0.2, 1)' : 'none',
                    '@keyframes mobileNavEnter': {
                        from: {
                            opacity: 0,
                            transform: 'translateX(-18px) scale(0.985)',
                        },
                        to: {
                            opacity: 1,
                            transform: 'translateX(0) scale(1)',
                        },
                    },
                    '&::before': {
                        content: '""',
                        position: 'absolute',
                        top: -80,
                        right: -80,
                        width: 220,
                        height: 220,
                        borderRadius: '50%',
                        background: 'radial-gradient(circle, rgba(13, 147, 201, 0.26) 0%, rgba(13, 147, 201, 0) 72%)',
                        pointerEvents: 'none',
                    },
                    '&::after': {
                        content: '""',
                        position: 'absolute',
                        bottom: -120,
                        left: -90,
                        width: 260,
                        height: 260,
                        borderRadius: '50%',
                        background: 'radial-gradient(circle, rgba(201, 168, 76, 0.2) 0%, rgba(201, 168, 76, 0) 74%)',
                        pointerEvents: 'none',
                    },
                },
            }}
            sx={{
                display: { lg: 'none' },
                '& .MuiBackdrop-root': {
                    backgroundColor: 'rgba(7, 16, 18, 0.48)',
                    backdropFilter: 'blur(1.5px)',
                    transition: 'all 0.28s ease',
                },
            }}
        >
            {/* Logo Section */}
            <Stack spacing={1} sx={{ p: 3, pb: 2, position: 'relative', zIndex: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', mb: 1 }}>
                    <img src="/images/TheGrandHoTram.png" alt="Logo" style={{ height: 80, width: 'auto' }} />
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
                        fontSize: '1.04rem',
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
                    {filteredNavItems.map((item, index) => (
                        <NavItemRenderer
                            key={item.key}
                            item={item}
                            openGroups={openGroups}
                            onToggleGroup={toggleGroup}
                            enableStagger={open}
                            staggerOrder={index}
                        />
                    ))}
                </List>
            </Box>
        </Drawer>
    );
}
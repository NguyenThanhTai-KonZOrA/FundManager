import * as React from 'react';
import { useLocation, Link as RouterLink } from 'react-router-dom';
import {
    Box,
    List,
    ListItem,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Collapse,
} from '@mui/material';
import {
    ExpandMore as ExpandMoreIcon,
    ExpandLess as ExpandLessIcon,
} from '@mui/icons-material';
import type { NavItem } from '../../types/commonType';

interface NavItemRendererProps {
    item: NavItem;
    depth?: number;
    openGroups: Record<string, boolean>;
    onToggleGroup: (key: string) => void;
    onItemClick?: () => void;
    enableStagger?: boolean;
    staggerOrder?: number;
}

export const NavItemRenderer: React.FC<NavItemRendererProps> = ({
    item,
    depth = 0,
    openGroups,
    onToggleGroup,
    onItemClick,
    enableStagger = false,
    staggerOrder = 0,
}) => {
    const location = useLocation();
    const Icon = item.icon;

    const hasActiveChild = (navItem: NavItem): boolean => {
        if (!navItem.children?.length) {
            return false;
        }

        return navItem.children.some((child) => {
            if (child.href === location.pathname) {
                return true;
            }
            return hasActiveChild(child);
        });
    };

    // If item has children, render as a group
    if (item.children && item.children.length > 0) {
        const isOpen = openGroups[item.key];
        const isGroupActive = hasActiveChild(item);
        const staggerDelay = (staggerOrder * 26) + (depth * 14);

        return (
            <React.Fragment key={item.key}>
                <ListItem disablePadding sx={{ mb: 0.5 }}>
                    <ListItemButton
                        onClick={() => onToggleGroup(item.key)}
                        sx={{
                            position: 'relative',
                            overflow: 'hidden',
                            borderRadius: 2,
                            color: isGroupActive ? '#ffffff' : 'rgba(255, 255, 255, 0.9)',
                            background: isGroupActive
                                ? 'linear-gradient(135deg, rgba(13, 147, 201, 0.22) 0%, rgba(255, 255, 255, 0.15) 100%), linear-gradient(120deg, rgba(201, 168, 76, 0.95) 0%, rgba(13, 147, 201, 0.95) 55%, rgba(201, 168, 76, 0.9) 100%)'
                                : 'rgba(255, 255, 255, 0.03)',
                            border: isGroupActive
                                ? '1px solid transparent'
                                : '1px solid transparent',
                            backgroundOrigin: isGroupActive ? 'border-box' : 'padding-box',
                            backgroundClip: isGroupActive ? 'padding-box, border-box' : 'padding-box',
                            boxShadow: isGroupActive
                                ? '0 10px 20px rgba(8, 22, 24, 0.26), inset 0 1px 0 rgba(255, 255, 255, 0.14)'
                                : 'none',
                            transition: 'all 0.22s ease',
                            animation: enableStagger
                                ? 'navItemStaggerIn 360ms cubic-bezier(0.2, 0.85, 0.24, 1) both'
                                : 'none',
                            animationDelay: enableStagger ? `${staggerDelay}ms` : '0ms',
                            '@keyframes navItemStaggerIn': {
                                from: {
                                    opacity: 0,
                                    transform: 'translateX(-8px)',
                                },
                                to: {
                                    opacity: 1,
                                    transform: 'translateX(0)',
                                },
                            },
                            '&:hover': {
                                background: isGroupActive
                                    ? 'linear-gradient(135deg, rgba(13, 147, 201, 0.3) 0%, rgba(255, 255, 255, 0.24) 100%), linear-gradient(120deg, rgba(201, 168, 76, 0.98) 0%, rgba(13, 147, 201, 1) 55%, rgba(201, 168, 76, 0.96) 100%)'
                                    : 'linear-gradient(135deg, rgba(13, 147, 201, 0.2) 0%, rgba(255, 255, 255, 0.13) 100%)',
                                color: 'white',
                                transform: depth === 0 ? 'translateX(3px)' : 'none',
                                borderColor: !isGroupActive ? 'rgba(173, 232, 255, 0.38)' : 'transparent',
                                '& .expand-icon': {
                                    opacity: 1,
                                }
                            },
                            py: 1.2,
                            px: 2,
                        }}
                    >
                        <ListItemIcon
                            sx={{
                                color: 'inherit',
                                minWidth: 40,
                            }}
                        >
                            <Icon fontSize="small" />
                        </ListItemIcon>
                        <ListItemText
                            primary={item.title}
                            primaryTypographyProps={{
                                fontSize: '0.9rem',
                                fontWeight: isGroupActive || isOpen ? 600 : 500,
                                whiteSpace: depth > 0 ? 'nowrap' : 'normal',
                                overflow: 'visible',
                                textOverflow: 'clip',
                                lineHeight: 1.25,
                            }}
                        />
                        <Box
                            className="expand-icon"
                            sx={{
                                display: 'flex',
                                alignItems: 'center',
                                opacity: isOpen || isGroupActive ? 1 : 0.6,
                                transition: 'opacity 0.2s ease'
                            }}
                        >
                            {isOpen ? <ExpandLessIcon fontSize="small" /> : <ExpandMoreIcon fontSize="small" />}
                        </Box>
                    </ListItemButton>
                </ListItem>
                <Collapse in={isOpen} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding>
                        {item.children.map((child, childIndex) => (
                            <NavItemRenderer
                                key={child.key}
                                item={child}
                                depth={depth + 1}
                                openGroups={openGroups}
                                onToggleGroup={onToggleGroup}
                                onItemClick={onItemClick}
                                enableStagger={enableStagger}
                                staggerOrder={staggerOrder + childIndex + 1}
                            />
                        ))}
                    </List>
                </Collapse>
            </React.Fragment>
        );
    }

    // Render as a single item
    const isActive = location.pathname === item.href;
    const paddingLeft = depth > 0 ? 2.4 + (depth * 1.1) : 2;
    const staggerDelay = (staggerOrder * 26) + (depth * 14);

    return (
        <ListItem key={item.key} disablePadding sx={{ mb: depth > 0 ? 0.5 : 1 }}>
            <ListItemButton
                component={RouterLink}
                to={item.href || '#'}
                onClick={onItemClick}
                sx={{
                    position: 'relative',
                    overflow: 'hidden',
                    borderRadius: 2,
                    color: isActive ? 'white' : 'rgba(255, 255, 255, 0.85)',
                    background: isActive
                        ? 'linear-gradient(135deg, rgba(13, 147, 201, 0.28) 0%, rgba(255, 255, 255, 0.2) 100%), linear-gradient(120deg, rgba(201, 168, 76, 0.98) 0%, rgba(13, 147, 201, 1) 55%, rgba(201, 168, 76, 0.94) 100%)'
                        : 'transparent',
                    border: isActive
                        ? '1px solid transparent'
                        : '1px solid transparent',
                    backgroundOrigin: isActive ? 'border-box' : 'padding-box',
                    backgroundClip: isActive ? 'padding-box, border-box' : 'padding-box',
                    boxShadow: isActive
                        ? '0 10px 18px rgba(8, 18, 20, 0.26), inset 0 1px 0 rgba(255, 255, 255, 0.14)'
                        : 'none',
                    transition: 'all 0.22s ease',
                    animation: enableStagger
                        ? 'navItemStaggerIn 360ms cubic-bezier(0.2, 0.85, 0.24, 1) both'
                        : 'none',
                    animationDelay: enableStagger ? `${staggerDelay}ms` : '0ms',
                    '@keyframes navItemStaggerIn': {
                        from: {
                            opacity: 0,
                            transform: 'translateX(-8px)',
                        },
                        to: {
                            opacity: 1,
                            transform: 'translateX(0)',
                        },
                    },
                    '&:hover': {
                        background: isActive
                            ? 'linear-gradient(135deg, rgba(13, 147, 201, 0.34) 0%, rgba(255, 255, 255, 0.26) 100%), linear-gradient(120deg, rgba(201, 168, 76, 1) 0%, rgba(13, 147, 201, 1) 55%, rgba(201, 168, 76, 0.98) 100%)'
                            : 'linear-gradient(135deg, rgba(13, 147, 201, 0.16) 0%, rgba(255, 255, 255, 0.1) 100%)',
                        borderColor: isActive ? 'transparent' : 'rgba(173, 232, 255, 0.32)',
                        transform: depth === 0 ? 'translateX(4px)' : 'none',
                    },
                    py: 1.2,
                    px: 2,
                    pl: paddingLeft,
                }}
            >
                <ListItemIcon
                    sx={{
                        color: 'inherit',
                        minWidth: depth > 0 ? 34 : 40,
                    }}
                >
                    <Icon fontSize="small" />
                </ListItemIcon>
                <ListItemText
                    primary={item.title}
                    primaryTypographyProps={{
                        fontSize: depth > 0 ? '0.78rem' : '0.9rem',
                        fontWeight: isActive ? 600 : 500,
                        whiteSpace: depth > 0 ? 'nowrap' : 'normal',
                        overflow: 'visible',
                        textOverflow: 'clip',
                        lineHeight: depth > 0 ? 1.2 : 1.25,
                    }}
                />
            </ListItemButton>
        </ListItem>
    );
};
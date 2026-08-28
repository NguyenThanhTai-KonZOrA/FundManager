export const UserRole = {
    ADMIN: 'Administrator',
    USER: 'user',
    OUTLET_STAFF: 'OutletStaff',
    TSO: 'TSO',
} as const;

export type UserRole = typeof UserRole[keyof typeof UserRole];

// System permissions definition
export const Permission = {
    CAN_VIEW_DASHBOARD: 'can_view_dashboard',
    CAN_VIEW_SIGNED_DOCUMENTS: 'can_view_signed_documents',
    CAN_VIEW_DEVICE_SETTING: 'can_view_device_settings',
    CAN_VIEW_SYSTEM_SETTING: 'can_view_system_settings',
} as const;

export type Permission = typeof Permission[keyof typeof Permission];

/**
 * Role to Permissions mapping
 * Define permissions for each role
 */
export const ROLE_PERMISSIONS: Record<UserRole, Permission[]> = {
    [UserRole.ADMIN]: [
        Permission.CAN_VIEW_DASHBOARD,
        Permission.CAN_VIEW_SIGNED_DOCUMENTS,
        Permission.CAN_VIEW_DEVICE_SETTING,
        Permission.CAN_VIEW_SYSTEM_SETTING,
    ],
    [UserRole.USER]: [
        Permission.CAN_VIEW_SIGNED_DOCUMENTS,
    ],
    [UserRole.OUTLET_STAFF]: [
        Permission.CAN_VIEW_SIGNED_DOCUMENTS,
    ],
    [UserRole.TSO]: [
        Permission.CAN_VIEW_DASHBOARD,
        Permission.CAN_VIEW_SIGNED_DOCUMENTS,
        Permission.CAN_VIEW_DEVICE_SETTING,
    ],
};

/**
 * Get permissions for a specific role
 */
export const getPermissionsForRole = (role: string | null): Permission[] => {
    if (!role) return [];

    const userRole = role as UserRole;
    return ROLE_PERMISSIONS[userRole] || [];
};

/**
 * Check if a role has a specific permission
 */
export const hasPermission = (role: string | null, permission: Permission): boolean => {
    const permissions = getPermissionsForRole(role);
    return permissions.includes(permission);
};

/**
 * Check if a role has any of the specified permissions
 */
export const hasAnyPermission = (role: string | null, permissions: Permission[]): boolean => {
    const userPermissions = getPermissionsForRole(role);
    return permissions.some(permission => userPermissions.includes(permission));
};

/**
 * Check if a role has all of the specified permissions
 */
export const hasAllPermissions = (role: string | null, permissions: Permission[]): boolean => {
    const userPermissions = getPermissionsForRole(role);
    return permissions.every(permission => userPermissions.includes(permission));
};

import {
    Settings as SettingsIcon,
    Dashboard as DashboardIcon,
    PersonAdd as PersonAddIcon,
    ManageHistory as ManageHistoryIcon,
    AssignmentInd as RoleManagementIcon,
    VerifiedUser as PermissionManagementIcon,
    People as PeopleIcon,
    Devices as DevicesIcon,
    Security as SecurityIcon,
    DynamicForm as DynamicFormIcon,
    AccountTree as WorkflowIcon,
    Description as DocumentTemplateIcon,
    Business as PropertyIcon,
    StoreMallDirectory as OutletIcon,
    Image as ApplicationImageIcon,
    Dataset as DatasetIcon,
} from '@mui/icons-material';
import type { NavItem } from '../../types/commonType';
import { Permission } from '../../constants/roles';

export const navItems: NavItem[] = [
    {
        key: 'admin-documents-signed',
        title: 'Documents Signed',
        href: '/admin-documents-signed',
        icon: PersonAddIcon,
        requiredPermission: Permission.CAN_VIEW_SIGNED_DOCUMENTS
    },
    {
        key: 'device-management',
        title: 'Device Management',
        icon: DevicesIcon,
        children: [
            {
                key: 'admin-device-mapping',
                title: 'Device Settings',
                href: '/admin-device-mapping',
                icon: SettingsIcon,
                requiredPermission: Permission.CAN_VIEW_DEVICE_SETTING,
            },
            {
                key: 'admin-devices',
                title: 'Device Mapping Report',
                href: '/admin-devices',
                icon: DevicesIcon,
                requiredPermission: Permission.CAN_VIEW_DEVICE_SETTING,
            },
        ]
    },
    {
        key: 'metadata',
        title: 'Metadata',
        icon: PropertyIcon,
        requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
        children: [
            {
                key: 'admin-master-data',
                title: 'Master Data Management',
                href: '/admin-master-data',
                icon: DatasetIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
            {
                key: 'admin-form-templates',
                title: 'Form Templates',
                href: '/admin-form-templates',
                icon: DynamicFormIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
            {
                key: 'admin-document-templates',
                title: 'Document Templates',
                href: '/admin-document-templates',
                icon: DocumentTemplateIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
            {
                key: 'admin-workflows',
                title: 'Workflows',
                href: '/admin-workflows',
                icon: WorkflowIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
            {
                key: 'admin-properties',
                title: 'Properties',
                href: '/admin-properties',
                icon: PropertyIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
            {
                key: 'admin-outlets',
                title: 'Outlets',
                href: '/admin-outlets',
                icon: OutletIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
            {
                key: 'admin-application-images',
                title: 'Application Images',
                href: '/admin-application-images',
                icon: ApplicationImageIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
        ],
    },
    {
        key: 'role-permission',
        title: 'Role & Permission',
        icon: SecurityIcon,
        children: [
            {
                key: 'admin-employees',
                title: 'Employee',
                href: '/admin-employees',
                icon: PeopleIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING
            },
            {
                key: 'admin-roles',
                title: 'Role',
                href: '/admin-roles',
                icon: RoleManagementIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING
            },
            {
                key: 'admin-permissions',
                title: 'Permission',
                href: '/admin-permissions',
                icon: PermissionManagementIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING
            },
        ]
    },
    {
        key: 'system',
        title: 'System',
        icon: SettingsIcon,
        requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
        children: [
            {
                key: 'admin-audit-logs',
                title: 'Audit Logs',
                href: '/admin-audit-logs',
                icon: ManageHistoryIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING,
            },
            {
                key: 'admin-settings',
                title: 'Settings',
                href: '/admin-settings',
                icon: SettingsIcon,
                requiredPermission: Permission.CAN_VIEW_SYSTEM_SETTING
            },
        ]
    },
];
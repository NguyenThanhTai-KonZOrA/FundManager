import type { Permission } from "../constants/roles";

export interface NavItem {
    key: string;
    title: string;
    href?: string;
    icon: React.ElementType;
    requiredPermission?: Permission;
    children?: NavItem[];
}

export type ApiEnvelope<T> = {
    status: number;
    data: T;
    success: boolean;
};

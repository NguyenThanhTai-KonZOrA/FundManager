// ── Outlet ────────────────────────────────────────────────────────────────────

export interface PropertyBrief {
    id: number;
    name: string;
    code: string;
    color: string;
    isPrimaryOutlet: boolean;
}

export interface OutletResponse {
    id: number;
    name: string;
    code: string;
    mainColor: string;
    description: string;
    iconImageUrl: string;
    backgroundImageUrl: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
    /** All properties this outlet belongs to (many-to-many). */
    properties: PropertyBrief[];
}

export interface CreateOutletRequest {
    Name: string;
    Code: string;
    Description: string;
    IconImageUrl: string;
    BackgroundImageUrl: string;
    /** IDs of properties this outlet belongs to (many-to-many) */
    PropertyIds: number[];
    MainColor: string;
}

export interface UpdateOutletRequest {
    Id: number;
    Name: string;
    Code: string;
    Description: string;
    IconImageUrl: string;
    BackgroundImageUrl: string;
    /** IDs of properties this outlet belongs to (many-to-many) */
    PropertyIds: number[];
    IsActive: boolean;
    MainColor: string;
}
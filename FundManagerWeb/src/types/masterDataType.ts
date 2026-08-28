// ─── Language ────────────────────────────────────────────────────────────────

export interface LanguageResponse {
    id: number;
    code: string;
    name: string;
    nativeName: string;
    flagEmoji?: string;
    sortOrder: number;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
    updatedBy: string;
}

export interface CreateLanguageRequest {
    Code: string;
    Name: string;
    NativeName: string;
    FlagEmoji?: string;
    SortOrder: number;
}

export interface UpdateLanguageRequest {
    Id: number;
    Code: string;
    Name: string;
    NativeName: string;
    FlagEmoji?: string;
    SortOrder: number;
}

// ─── PatronType ───────────────────────────────────────────────────────────────

export interface PatronTypeResponse {
    id: number;
    name: string;
    colorHex?: string;
    description?: string;
    sortOrder: number;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
    updatedBy: string;
}

export interface CreatePatronTypeRequest {
    Name: string;
    ColorHex?: string;
    Description?: string;
    SortOrder: number;
}

export interface UpdatePatronTypeRequest {
    Id: number;
    Name: string;
    ColorHex?: string;
    Description?: string;
    SortOrder: number;
}

export interface SettingsResponse {
    id: number;
    key: string;
    value: string;
    description: string;
    category: string;
    createdAt: string;
    createdBy: string;
    updatedAt: string;
    updatedBy: string;
    isActive: boolean;
    dataType: string;
}

export interface CreateSettingsRequest {
    key: string;
    value: string;
    description: string;
    category: string;
    dataType: string;
}

export interface SettingsInfoResponse {
    cacheExpirationMinutes: number;
    categories: CategoriesofSettingsResponse[],
    dataType: []
}

export interface CategoriesofSettingsResponse {
    name: string;
    description: string;
    appliesImmediately: boolean;
}

export interface ClearCacheSettingResponse {
    message: string;
    key: string;
}

export interface UpdateSettingsRequest {
    key: string;
    value: string;
}

export interface UpdateSettingsResponse {
    key: string;
    value: string;
    requiresRestart: boolean;
    warning: string;
    appliedAt: string;
}
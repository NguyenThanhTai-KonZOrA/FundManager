// ── Property ──────────────────────────────────────────────────────────────────

import type { OutletResponse } from "./outletType";

export interface PropertyResponse {
    id: number;
    name: string;
    code: string;
    description: string;
    color: string;
    isActive: boolean;
    isPrimaryOutlet: boolean;
    createdAt: string;
    updatedAt: string;
    outlets: OutletResponse[];
}

export interface CreatePropertyRequest {
    Name: string;
    Description: string;
    Color: string;
    OutletIds: number[];
}

export interface UpdatePropertyRequest {
    Id: number;
    Name: string;
    Description: string;
    Color: string;
    IsActive: boolean;
    OutletIds: number[];
}

// ── Device ───────────────────────────────────────────────────────────────────

export interface DeviceResponse {
    id: number;
    deviceName: string;
    macAddress: string;
    ipAddress: string | null;
    staffUserName: string | null;
    isOnline: boolean;
    lastHeartbeat: string | null;
    isActive: boolean;
}

export interface RegisterDeviceRequest {
    DeviceName: string;
    MacAddress: string;
    IpAddress?: string;
    StaffUserName?: string;
}

// ── PropertyDeviceMapping ────────────────────────────────────────────────────

export interface PropertyDeviceMappingResponse {
    id: number;
    deviceId: number;
    deviceName: string;
    macAddress: string;
    ipAddress: string | null;
    propertyId: number;
    propertyName: string;
    propertyCode: string;
    propertyColor: string;
    location: string | null;
    notes: string | null;
    lastVerified: string | null;
    outlets: OutletResponse[];
}

export interface CreateMappingRequest {
    DeviceId: number;
    PropertyId: number;
    Location?: string;
    Notes?: string;
}

export interface UpdateMappingRequest {
    Id: number;
    DeviceId: number;
    PropertyId: number;
    Location?: string;
    Notes?: string;
}

export interface GetPropertyDeviceMappedRequest {
    DeviceName?: string;
    MacAddress?: string;
}

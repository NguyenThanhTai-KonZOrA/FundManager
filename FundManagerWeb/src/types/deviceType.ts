export interface CurrentStaffDeviceResponse {
    id: number;
    staffDeviceId: number;
    deviceName: string;
    hostName: string;
    location: string;
    assignedStaffId: string;
    connectionId: string;
}

export interface StaffSignatureRequest {
    PatronId: number;
}

export interface CreateMappingRequest {
    StaffDeviceName: string;
    PatronDeviceName: string;
    OutletId: number;
    Notes: string;
}

export interface CreateMappingResponse {
    id: number;
    staffDeviceId: number;
    staffDeviceName: string;
    patronDeviceId: number;
    patronDeviceName: string;
    location: string;
    isActive: boolean;
    lastVerified: string;
    outletId: number;
    outletName: string;
}

export interface MappingDataResponse {
    outletId: number;
    outletName: string;
    id: number;
    staffDeviceId: number;
    staffDeviceName: string;
    staffIp: string;
    staffDeviceIsOnline: boolean;
    patronDeviceId: number;
    patronDeviceName: string;
    patronIp: string;
    patronIsOnline: boolean;
    staffIsOnline: boolean;
    location: string;
    propertyName: string;
    propertyId: number;
    notes: string;
    isActive: boolean;
    lastVerified: string;
    createdAt: string;
}

export interface GetMappingByStaffDeviceResponse {
    id: number;
    staffDeviceId: number;
    staffDeviceName: string;
    patronDeviceId: number;
    patronDeviceName: string;
    location: string;
    isActive: boolean;
    lastVerified: string;
}

export interface UpdateMappingRequest {
    Id: number;
    NewStaffDeviceName: string;
    NewPatronDeviceName: string;
    Location: string;
    Notes: string;
    OutletId: number;
    OutletName: string;
}

export interface UpdateMappingResponse {
    id: number;
    staffDeviceId: number;
    staffDeviceName: string;
    patronDeviceId: number;
    patronDeviceName: string;
    location: string;
    notes: string;
    isActive: boolean;
    lastVerified: string;
    updatedAt: string;
}

export interface StaffAndPatronDevicesResponse {
    staffDevices: StaffDeviceResponse[];
    patronDevices: PatronDeviceResponse[];
}

export interface StaffDeviceResponse {
    staffDeviceId: number;
    staffDeviceName: string;
}

export interface PatronDeviceResponse {
    patronDeviceId: number;
    patronDeviceName: string;
}

export interface OnlineStaffDevicesResponse {
    id: number;
    deviceName: string;
    connectionId: string;
    isOnline: boolean;
    ipAddress: string;
    staffUserName: string;
    lastHeartbeat: string;
}

export interface CurrentHostNameResponse {
    computerName: string;
    ip: string;
}

export interface GetAllMappingsResponse {
    count: number;
    data: MappingDataResponse[];
}

export interface ManageDeviceResponse {
    patronDevices: DeviceInfo[];
    staffDevices: DeviceInfo[];
    totalPatronDevices: number;
    totalStaffDevices: number;
}

export interface DeviceInfo {
    id: number;
    deviceName: string;
    deviceType: string; // "Staff" or "Patron"
    macAddress: string;
    ipAddress: string;
    isOnline: boolean;
    isActive: boolean;
    connectionId: string;
    staffUserName: string;
    lastHeartbeat: string;
    createdAt: string;
    updatedAt: string;
    status: boolean;
    lastActiveAt: string;
}

export interface ToggleDeviceRequest {
    deviceId: number;
    deviceType: string; // "Staff" or "Patron"
    isActive: boolean;
}

export interface DeleteDeviceRequest {
    deviceId: number;
    deviceType: string; // "Staff" or "Patron"
}

export interface ChangeHostnameRequest {
    deviceId: number;
    deviceType: string; // "Staff" or "Patron"
    newHostname: string;
}
export interface AuditLogPaginationRequest {
    Page: number;
    PageSize: number;
    Take?: number;
    Skip?: number;
    FromDate?: string;
    ToDate?: string;
    Action?: string;
    UserName?: string;
    IsSuccess?: boolean;
    EntityType?: string;
}

export interface AuditLogPaginationResponse {
    page: number;
    pageSize: number;
    totalRecords: number;
    logs: AuditLogResponse[];
}

export interface AuditLogResponse {
    id: number;
    userName: string;
    action: string;
    entityType: string;
    httpMethod: string;
    requestPath: string;
    ipAddress: string;
    userAgent: string;
    isSuccess: boolean;
    statusCode: number;
    errorMessage: string;
    details: string;
    createdAt: string;
    entityId: string;
    timestamp: string;
}

export interface AuditLogsRegisterMembershipRequest {
    PlayerId: number;
    ActionType: string;
    MembershipStatus: string;
    FromDate: string;
    ToDate: string;
    UserName?: string;
    Page: number;
    PageSize: number;
    Take?: number;
    Skip?: number;
}

export interface AuditLogsRegisterMembershipPaginationResponse {
    totalRecords: number;
    page: number;
    pageSize: number;
    logs: AuditLogsRegisterMembershipResponse[];
}

export interface AuditLogsRegisterMembershipResponse {
    id: number;
    actionType: string;
    employeeId: number;
    employeeName: string;
    employeeCode: string;
    actionDate: string;
    playerId: number;
    membershipStatus: string;
    isSuccess: boolean;
    errorMessage: string;
    details: string;
    playerName: string;
}
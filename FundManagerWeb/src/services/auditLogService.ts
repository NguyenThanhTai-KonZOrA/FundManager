import type { AuditLogPaginationRequest, AuditLogPaginationResponse, AuditLogResponse, AuditLogsRegisterMembershipRequest, AuditLogsRegisterMembershipPaginationResponse } from "../types/auditLogsType";
import type { ApiEnvelope } from "../types/commonType";
import { api, unwrapApiEnvelope } from "./commonApiService";
import { BASE_AUDIT_LOG } from "../constants/baseApiEnpoint";

export const auditLogService = {
    getAuditLogsFilterOptionsAsync: async (request: AuditLogPaginationRequest): Promise<AuditLogPaginationResponse> => {
        const response = await api.post<ApiEnvelope<AuditLogPaginationResponse>>(`${BASE_AUDIT_LOG}/paginate`, request);
        return unwrapApiEnvelope(response);
    },

    getAuditLogByIdAsync: async (auditLogId: number): Promise<AuditLogResponse> => {
        const response = await api.get<ApiEnvelope<AuditLogResponse>>(`${BASE_AUDIT_LOG}/${auditLogId}`);
        return unwrapApiEnvelope(response);
    },

    getRegisteredLogsAsync: async (request: AuditLogsRegisterMembershipRequest): Promise<AuditLogsRegisterMembershipPaginationResponse> => {
        const response = await api.post<ApiEnvelope<AuditLogsRegisterMembershipPaginationResponse>>(`${BASE_AUDIT_LOG}/audit-logs-membership/paginate`, request);
        return unwrapApiEnvelope(response);
    },
};
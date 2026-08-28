import type { ApiEnvelope } from '../types/commonType';
import type {
    SignedCustomerListRequest,
    SignedCustomerListResponse,
    SignedCustomerRow,
    SessionPrefillResponse,
    FormTemplateVersionHistoryResponse,
    FormTemplateWithQuestionsResponse,
    DocumentTemplateDetailResponse,
} from '../types/customerSignedType';
import { api, unwrapApiEnvelope } from './commonApiService';
import { BASE_CUSTOMER_SIGNED } from '../constants/baseApiEnpoint';
import type {
    DocumentTemplateTranslationResponse, DocumentTemplateVersionHistoryResponse,
    UpsertDocumentTemplateTranslationRequest,
} from '../types/documentTemplateType';
import type { FormTemplateTranslationResponse, UpsertFormTemplateTranslationRequest } from '../types/formTemplateType';

export const customerSignedService = {

    // ─── Admin: Signed customers ─────────────────────────────────────────

    /** GET /api/customer-sign/admin/signed-customers (server-paged, filtered) */
    getSignedCustomersAsync: async (req: SignedCustomerListRequest): Promise<SignedCustomerListResponse> => {
        const params = new URLSearchParams();
        params.append('page', String(req.page));
        params.append('pageSize', String(req.pageSize));
        if (req.searchTerm) params.append('searchTerm', req.searchTerm);
        if (req.fromDate) params.append('fromDate', req.fromDate);
        if (req.toDate) params.append('toDate', req.toDate);
        if (req.outletId) params.append('outletId', String(req.outletId));
        if (req.patronTypeId) params.append('patronTypeId', String(req.patronTypeId));
        if (req.customerType) params.append('customerType', req.customerType);

        const res = await api.get<ApiEnvelope<SignedCustomerListResponse>>(
            `${BASE_CUSTOMER_SIGNED}/admin/signed-customers?${params.toString()}`
        );
        return unwrapApiEnvelope(res);
    },

    /** GET /api/customer-sign/admin/signed-customers/{patronId} */
    getSignedCustomerDetailAsync: async (patronId: number): Promise<SignedCustomerRow> => {
        const res = await api.get<ApiEnvelope<SignedCustomerRow>>(
            `${BASE_CUSTOMER_SIGNED}/admin/signed-customers/${patronId}`
        );
        return unwrapApiEnvelope(res);
    },

    /**
     * GET /api/customer-sign/admin/session-prefill/{patronId}
     * Loads previous patron data so the iPad can pre-fill the form for duplicate signing.
     */
    getSessionPrefillAsync: async (patronId: number): Promise<SessionPrefillResponse> => {
        const res = await api.get<ApiEnvelope<SessionPrefillResponse>>(
            `${BASE_CUSTOMER_SIGNED}/admin/session-prefill/${patronId}`
        );
        return unwrapApiEnvelope(res);
    },

    // ─── Template translations ────────────────────────────────────────────

    getFormTemplateTranslationsAsync: async (formTemplateId: number): Promise<FormTemplateTranslationResponse[]> => {
        const res = await api.get<ApiEnvelope<FormTemplateTranslationResponse[]>>(
            `${BASE_CUSTOMER_SIGNED}/form-templates/${formTemplateId}/translations`
        );
        return unwrapApiEnvelope(res);
    },

    upsertFormTemplateTranslationAsync: async (req: UpsertFormTemplateTranslationRequest): Promise<FormTemplateTranslationResponse> => {
        const res = await api.put<ApiEnvelope<FormTemplateTranslationResponse>>(
            `${BASE_CUSTOMER_SIGNED}/form-templates/translations`, req
        );
        return unwrapApiEnvelope(res);
    },

    getDocumentTemplateTranslationsAsync: async (documentTemplateId: number): Promise<DocumentTemplateTranslationResponse[]> => {
        const res = await api.get<ApiEnvelope<DocumentTemplateTranslationResponse[]>>(
            `${BASE_CUSTOMER_SIGNED}/document-templates/${documentTemplateId}/translations`
        );
        return unwrapApiEnvelope(res);
    },

    upsertDocumentTemplateTranslationAsync: async (req: UpsertDocumentTemplateTranslationRequest): Promise<DocumentTemplateTranslationResponse> => {
        const res = await api.put<ApiEnvelope<DocumentTemplateTranslationResponse>>(
            `${BASE_CUSTOMER_SIGNED}/document-templates/translations`, req
        );
        return unwrapApiEnvelope(res);
    },

    // ─── Version histories ───────────────────────────────────────────────

    getFormTemplateHistoryAsync: async (formTemplateId: number): Promise<FormTemplateVersionHistoryResponse[]> => {
        const res = await api.get<ApiEnvelope<FormTemplateVersionHistoryResponse[]>>(
            `${BASE_CUSTOMER_SIGNED}/form-templates/${formTemplateId}/history`
        );
        return unwrapApiEnvelope(res);
    },

    getDocumentTemplateHistoryAsync: async (documentTemplateId: number): Promise<DocumentTemplateVersionHistoryResponse[]> => {
        const res = await api.get<ApiEnvelope<DocumentTemplateVersionHistoryResponse[]>>(
            `${BASE_CUSTOMER_SIGNED}/document-templates/${documentTemplateId}/history`
        );
        return unwrapApiEnvelope(res);
    },

    // ─── Kiosk: form/document template for the iPad ────────────────────────────

    getFormTemplateAsync: async (formTemplateId: number): Promise<FormTemplateWithQuestionsResponse> => {
        const res = await api.get(`${BASE_CUSTOMER_SIGNED}/form-template/${formTemplateId}`);
        return unwrapApiEnvelope(res);
    },

    getDocumentTemplateAsync: async (documentTemplateId: number): Promise<DocumentTemplateDetailResponse> => {
        const res = await api.get(`${BASE_CUSTOMER_SIGNED}/document-template/${documentTemplateId}`);
        return unwrapApiEnvelope(res);
    },

    // ─── Legacy alias (kept for any existing callers) ────────────────────────────
    getDocumentSignedListAsync: async (req: SignedCustomerListRequest): Promise<SignedCustomerListResponse> =>
        customerSignedService.getSignedCustomersAsync(req),
};

import { api, unwrapApiEnvelope } from './commonApiService';
import type {
    DocumentTemplateBriefResponse,
    DocumentTemplateResponse,
    CreateDocumentTemplateRequest,
    UpdateDocumentTemplateRequest,
} from '../types/documentTemplateType';
import { DocumentType } from '../types/documentTemplateType';
import { BASE_DOCUMENT } from '../constants/baseApiEnpoint';
import type {
    DocumentTemplateTranslationResponse,
    DocumentTemplateVersionHistoryResponse,
    UpsertDocumentTemplateTranslationRequest,
} from '../types/documentTemplateType';

export const documentTemplateService = {
    getListAsync: async (): Promise<DocumentTemplateBriefResponse[]> => {
        const res = await api.get(`${BASE_DOCUMENT}/list`);
        return unwrapApiEnvelope(res);
    },

    getByIdAsync: async (id: number): Promise<DocumentTemplateResponse> => {
        const res = await api.get(`${BASE_DOCUMENT}/${id}`);
        return unwrapApiEnvelope(res);
    },

    getByTypeAsync: async (documentType: DocumentType): Promise<DocumentTemplateBriefResponse[]> => {
        const res = await api.get(`${BASE_DOCUMENT}/by-type/${documentType}`);
        return unwrapApiEnvelope(res);
    },

    getByOutletAsync: async (outletId: number): Promise<DocumentTemplateBriefResponse[]> => {
        const res = await api.get(`${BASE_DOCUMENT}/by-outlet/${outletId}`);
        return unwrapApiEnvelope(res);
    },

    createAsync: async (req: CreateDocumentTemplateRequest): Promise<DocumentTemplateResponse> => {
        const res = await api.post(`${BASE_DOCUMENT}/create`, req);
        return unwrapApiEnvelope(res);
    },

    updateAsync: async (req: UpdateDocumentTemplateRequest): Promise<DocumentTemplateResponse> => {
        const res = await api.post(`${BASE_DOCUMENT}/update`, req);
        return unwrapApiEnvelope(res);
    },

    deleteAsync: async (id: number): Promise<void> => {
        const res = await api.post(`${BASE_DOCUMENT}/delete/${id}`);
        return unwrapApiEnvelope(res);
    },

    // ─── Translations ─────────────────────────────────────────────────────────
    getTranslationsAsync: async (documentTemplateId: number): Promise<DocumentTemplateTranslationResponse[]> => {
        const res = await api.get(`${BASE_DOCUMENT}/${documentTemplateId}/translations`);
        return unwrapApiEnvelope(res);
    },

    upsertTranslationAsync: async (req: UpsertDocumentTemplateTranslationRequest): Promise<DocumentTemplateTranslationResponse> => {
        const res = await api.post(`${BASE_DOCUMENT}/translations`, req);
        return unwrapApiEnvelope(res);
    },

    // ─── Version History ─────────────────────────────────────────────────────
    getHistoryAsync: async (documentTemplateId: number): Promise<DocumentTemplateVersionHistoryResponse[]> => {
        const res = await api.get(`${BASE_DOCUMENT}/${documentTemplateId}/history`);
        return unwrapApiEnvelope(res);
    },
};

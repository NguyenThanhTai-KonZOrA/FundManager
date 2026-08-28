import { api, unwrapApiEnvelope } from './commonApiService';
import { BASE_SUBMISSION, BASE_TEMPLATE, BASE_WORKFLOW } from '../constants/baseApiEnpoint';
import type {
    WorkflowResponse,
    CreateWorkflowRequest,
    UpdateWorkflowRequest,
} from '../types/formWorkflowType';
import type {
    CreateFormQuestionRequest, CreateFormTemplateRequest,
    FormQuestionResponse, FormSubmissionBriefResponse,
    FormSubmissionResponse, FormTemplateBriefResponse,
    FormTemplateResponse, FormTemplateTranslationResponse,
    FormTemplateVersionHistoryResponse, ReorderQuestionsRequest,
    UpdateFormQuestionRequest, UpdateFormTemplateRequest,
    UpsertFormTemplateTranslationRequest
} from '../types/formTemplateType';

// ─── Form Template ────────────────────────────────────────────────────────────
export const formTemplateService = {
    getListAsync: async (): Promise<FormTemplateBriefResponse[]> => {
        const res = await api.get(`${BASE_TEMPLATE}/list`);
        return unwrapApiEnvelope(res);
    },

    getByIdAsync: async (id: number): Promise<FormTemplateResponse> => {
        const res = await api.get(`${BASE_TEMPLATE}/${id}`);
        return unwrapApiEnvelope(res);
    },

    createAsync: async (req: CreateFormTemplateRequest): Promise<FormTemplateResponse> => {
        const res = await api.post(`${BASE_TEMPLATE}/create`, req);
        return unwrapApiEnvelope(res);
    },

    updateAsync: async (req: UpdateFormTemplateRequest): Promise<FormTemplateResponse> => {
        const res = await api.post(`${BASE_TEMPLATE}/update`, req);
        return unwrapApiEnvelope(res);
    },

    deleteAsync: async (id: number): Promise<void> => {
        const res = await api.post(`${BASE_TEMPLATE}/delete/${id}`);
        return unwrapApiEnvelope(res);
    },

    addQuestionAsync: async (req: CreateFormQuestionRequest): Promise<FormQuestionResponse> => {
        const res = await api.post(`${BASE_TEMPLATE}/question/add`, req);
        return unwrapApiEnvelope(res);
    },

    updateQuestionAsync: async (req: UpdateFormQuestionRequest): Promise<FormQuestionResponse> => {
        const res = await api.post(`${BASE_TEMPLATE}/question/update`, req);
        return unwrapApiEnvelope(res);
    },

    deleteQuestionAsync: async (questionId: number): Promise<void> => {
        const res = await api.post(`${BASE_TEMPLATE}/question/delete/${questionId}`);
        return unwrapApiEnvelope(res);
    },

    reorderQuestionsAsync: async (req: ReorderQuestionsRequest): Promise<void> => {
        const res = await api.post(`${BASE_TEMPLATE}/question/reorder`, req);
        return unwrapApiEnvelope(res);
    },

    // ─── Translations ─────────────────────────────────────────────────────────
    getTranslationsAsync: async (formTemplateId: number): Promise<FormTemplateTranslationResponse[]> => {
        const res = await api.get(`${BASE_TEMPLATE}/${formTemplateId}/translations`);
        return unwrapApiEnvelope(res);
    },

    upsertTranslationAsync: async (req: UpsertFormTemplateTranslationRequest): Promise<FormTemplateTranslationResponse> => {
        const res = await api.post(`${BASE_TEMPLATE}/translations`, req);
        return unwrapApiEnvelope(res);
    },

    // ─── Version History ──────────────────────────────────────────────────────
    getHistoryAsync: async (formTemplateId: number): Promise<FormTemplateVersionHistoryResponse[]> => {
        const res = await api.get(`${BASE_TEMPLATE}/${formTemplateId}/history`);
        return unwrapApiEnvelope(res);
    },
};

// ─── Form Submission ──────────────────────────────────────────────────────────
export const formSubmissionService = {
    getByIdAsync: async (id: number): Promise<FormSubmissionResponse> => {
        const res = await api.get(`${BASE_SUBMISSION}/${id}`);
        return unwrapApiEnvelope(res);
    },

    getByTemplateAsync: async (templateId: number): Promise<FormSubmissionBriefResponse[]> => {
        const res = await api.get(`${BASE_SUBMISSION}/by-template/${templateId}`);
        return unwrapApiEnvelope(res);
    },

    getByPatronDeviceAsync: async (patronDeviceId: number): Promise<FormSubmissionBriefResponse[]> => {
        const res = await api.get(`${BASE_SUBMISSION}/patron-device/${patronDeviceId}`);
        return unwrapApiEnvelope(res);
    },
};

// ─── Workflow ─────────────────────────────────────────────────────────────────

export const workflowService = {
    getListAsync: async (): Promise<WorkflowResponse[]> => {
        const res = await api.get(`${BASE_WORKFLOW}/list`);
        return unwrapApiEnvelope(res);
    },

    getByIdAsync: async (id: number): Promise<WorkflowResponse> => {
        const res = await api.get(`${BASE_WORKFLOW}/${id}`);
        return unwrapApiEnvelope(res);
    },

    createAsync: async (req: CreateWorkflowRequest): Promise<WorkflowResponse> => {
        const res = await api.post(`${BASE_WORKFLOW}/create`, req);
        return unwrapApiEnvelope(res);
    },

    updateAsync: async (req: UpdateWorkflowRequest): Promise<WorkflowResponse> => {
        const res = await api.post(`${BASE_WORKFLOW}/update`, req);
        return unwrapApiEnvelope(res);
    },

    deleteAsync: async (id: number): Promise<void> => {
        const res = await api.post(`${BASE_WORKFLOW}/delete/${id}`);
        return unwrapApiEnvelope(res);
    },
};

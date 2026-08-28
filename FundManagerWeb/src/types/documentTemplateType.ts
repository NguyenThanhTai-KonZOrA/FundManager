// ─── Enums ───────────────────────────────────────────────────────────────────

export const DocumentType = {
    PDP: 1,
    HTP: 2,
    Term: 3,
    SpaAcknowledgement: 4,
    Other: 99,
} as const;

export type DocumentType = typeof DocumentType[keyof typeof DocumentType];

export const DOCUMENT_TYPE_LABELS: Record<number, string> = {
    [DocumentType.PDP]: 'Personal Data Processing (PDP)',
    [DocumentType.HTP]: 'Hotel Terms & Policies (HTP)',
    [DocumentType.Term]: 'Terms & Conditions',
    [DocumentType.SpaAcknowledgement]: 'Spa Acknowledgement',
    [DocumentType.Other]: 'Other',
};

// ─── Response DTOs ─────────────────────────────────────────────────────────────

export interface DocumentTemplateBriefResponse {
    id: number;
    title: string;
    documentType: DocumentType;
    documentTypeName: string;
    description: string;
    version: number;
    isActive: boolean;
    outletId: number | null;
    outletName: string | null;
    updatedAt: string;
    updatedBy: string;
    translations: DocumentTemplateTranslationResponse[];
    versionHistories: DocumentTemplateVersionHistoryResponse[];
}

export interface DocumentTemplateResponse {
    id: number;
    title: string;
    documentType: DocumentType;
    documentTypeName: string;
    description: string;
    content: string;
    version: number;
    isActive: boolean;
    outletId: number | null;
    outletName: string | null;
    createdAt: string;
    updatedAt: string;
    translations: DocumentTemplateTranslationResponse[];
    versionHistories: DocumentTemplateVersionHistoryResponse[];
}

// ─── Request DTOs ──────────────────────────────────────────────────────────────

export interface CreateDocumentTemplateRequest {
    title: string;
    documentType: DocumentType;
    description: string;
    content: string;
    outletId: number | null;
}

export interface UpdateDocumentTemplateRequest {
    id: number;
    title: string;
    documentType: DocumentType;
    description: string;
    content: string;
    outletId: number | null;
    isActive: boolean;
}

// ─── Template Translations ───────────────────────────────────────────────────

export interface DocumentTemplateTranslationResponse {
    id: number;
    documentTemplateId: number;
    languageCode: string;
    title: string;
    description?: string;
    content: string;
    updatedAt: string;
    updatedBy: string;
}

export interface UpsertDocumentTemplateTranslationRequest {
    documentTemplateId: number;
    languageCode: string;
    title: string;
    description?: string;
    content: string;
}

export interface DocumentTemplateVersionHistoryResponse {
    id: number;
    documentTemplateId: number;
    version: number;
    title: string;
    description: string;
    content: string;
    updatedAt: string;
    updatedBy: string;
    changeNote?: string;
}
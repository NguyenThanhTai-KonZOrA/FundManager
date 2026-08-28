// ─── Signed Customers List (Admin) ─────────────────────────────────────────

export interface SignedCustomerListRequest {
    page: number;
    pageSize: number;
    searchTerm?: string;
    fromDate?: string;   // ISO date string
    toDate?: string;
    outletId?: number;
    patronTypeId?: number;
    customerType?: string; // 'InHouse' | 'WalkIn'
}

export interface SignedCustomerListResponse {
    totalRecords: number;
    data: SignedCustomerRow[];
}

export interface SignedCustomerRow {
    id: number;
    displayId: string;      // e.g. "#G-2481"
    firstName?: string;
    lastName?: string;
    customerName: string;
    email?: string;
    patronType?: string;
    patronTypeColor?: string;
    roomNumber?: string;
    language?: string;
    customerType?: string;
    outletId?: number;
    outletName?: string;
    signedAt: string;       // ISO datetime
    signedBy?: string;
    signedByDevice?: string;
    documents: SignedDocumentRow[];
    phoneNumber?: string;
    nationality?: string;
}

export interface SignedDocumentRow {
    patronSignatureId: number;
    documentTypeName: string;
    fileName: string;
    fileUrl?: string;
    signedAt: string;
    status: string;
    signedByDevice?: string;
}

// ─── Session Prefill (iPad reload) ──────────────────────────────────────────

export interface SessionPrefillResponse {
    patronId: number;
    firstName?: string;
    lastName?: string;
    roomNumber?: string;
    language?: string;
    customerType?: string;
    nationality?: string;
    phoneNumber?: string;
    idPassport?: string;
    previousAnswers: PrefillAnswer[];
}

export interface PrefillAnswer {
    formQuestionId: number;
    answerValue?: string;
    followUpText?: string;
}

// ─── Version Histories ───────────────────────────────────────────────────────

export interface FormTemplateVersionHistoryResponse {
    id: number;
    formTemplateId: number;
    version: number;
    title: string;
    description: string;
    footerText?: string;
    questionsSnapshot: string;
    updatedAt: string;
    updatedBy: string;
    changeNote?: string;
}

// ─── Legacy types kept for backward compatibility ────────────────────────────

export type CustomerSignedPaginatedResponse = SignedCustomerListResponse;
export type CustomerSignedResponse = SignedCustomerRow;
export type DocumentResponse = SignedDocumentRow;
export interface CustomerSignedRequest extends SignedCustomerListRequest { }

export interface DocumentTemplateDetailResponse {
    id: number;
    title: string;
    documentType: number;
    documentTypeName: string;
    description: string;
    content: string;
    version: number;
}

export interface FormTemplateWithQuestionsResponse {
    id: number;
    title: string;
    description: string;
    logoUrl?: string;
    footerText?: string;
    version: number;
    questions: FormQuestionResponse[];
}

export interface FormQuestionResponse {
    id: number;
    formTemplateId: number;
    sortOrder: number;
    questionText: string;
    questionType: number;
    isRequired: boolean;
    hasFollowUpText: boolean;
    followUpLabel?: string;
    followUpTriggerOption?: string;
    options: FormQuestionOptionResponse[];
}

export interface FormQuestionOptionResponse {
    id: number;
    optionText: string;
    sortOrder: number;
}

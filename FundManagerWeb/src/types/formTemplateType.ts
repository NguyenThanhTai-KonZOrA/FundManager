// ─── Enums ───────────────────────────────────────────────────────────────────
export const QuestionType = {
    'TextInput': 1,
    'SingleChoice': 2,
    'MultipleChoice': 3,
    'YesNo': 4,
    'Rating': 5,
    'Date': 6,
} as const;
export type QuestionType = typeof QuestionType[keyof typeof QuestionType];


// ─── Translation & Version History ─────────────────────────────────────────────

export interface FormTemplateTranslationResponse {
    id: number;
    formTemplateId: number;
    languageCode: string;
    title: string;
    description: string | null;
    footerText: string | null;
    agreementText: string;
    questionsTranslation: string | null; // JSON string
    updatedAt: string;
    updatedBy: string;
}

export interface FormTemplateVersionHistoryResponse {
    id: number;
    formTemplateId: number;
    version: number;
    title: string;
    description: string;
    footerText: string | null;
    agreementText: string;
    questionsSnapshot: string; // JSON string
    updatedAt: string;
    updatedBy: string;
    changeNote: string | null;
}

export interface UpsertFormTemplateTranslationRequest {
    formTemplateId: number;
    languageCode: string;
    title: string;
    description: string | null;
    footerText: string | null;
    agreementText: string;
    questionsTranslation: string | null;
}

export interface TransQuestionsDraftData {
    questionId: number;
    questionText: string;
    followUpLabel: string;
    hasFollowUpText: boolean;
    options: OptionTranslationDraftData[];
}

export interface OptionTranslationDraftData {
    optionId: number;
    optionText: string;
}

// ─── Form Template ────────────────────────────────────────────────────────────

export interface FormTemplateBriefResponse {
    id: number;
    title: string;
    description: string;
    logoUrl: string;
    version: number;
    isActive: boolean;
    updatedAt: string;
    updatedBy: string;
    translations: FormTemplateTranslationResponse[];
    versionHistories: FormTemplateVersionHistoryResponse[];
}

export interface FormQuestionOptionResponse {
    id: number;
    optionText: string;
    sortOrder: number;
}

export interface FormQuestionResponse {
    id: number;
    formTemplateId: number;
    sortOrder: number;
    questionText: string;
    questionType: number;
    isRequired: boolean;
    hasFollowUpText: boolean;
    followUpLabel: string | null;
    followUpTriggerOption: string | null;
    options: FormQuestionOptionResponse[];
}

export interface FormTemplateResponse {
    id: number;
    title: string;
    description: string;
    logoUrl: string;
    footerText: string;
    agreementText: string;
    version: number;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
    questions: FormQuestionResponse[];
}

// ─── Form Template Requests ────────────────────────────────────────────────────

export interface CreateFormTemplateRequest {
    title: string;
    description: string;
    logoUrl: string;
    footerText: string;
    agreementText: string;
}

export interface UpdateFormTemplateRequest {
    id: number;
    title: string;
    description: string;
    logoUrl: string;
    footerText: string;
    agreementText: string;
    isActive: boolean;
}

export interface CreateFormQuestionRequest {
    formTemplateId: number;
    questionText: string;
    questionType: number;
    isRequired: boolean;
    hasFollowUpText: boolean;
    followUpLabel: string | null;
    followUpTriggerOption: string | null;
    options: string[];
}

export interface UpdateFormQuestionRequest {
    id: number;
    questionText: string;
    questionType: number;
    questionTypeName?: string;
    isRequired: boolean;
    hasFollowUpText: boolean;
    followUpLabel: string | null;
    followUpTriggerOption: string | null;
    options: string[];
}

export interface ReorderQuestionsRequest {
    formTemplateId: number;
    questionIds: number[];
}

// ─── Form Submission ──────────────────────────────────────────────────────────

export interface SubmitAnswerRequest {
    formQuestionId: number;
    answerValue: string;
    followUpText: string | null;
}

export interface SubmitFormRequest {
    formTemplateId: number;
    patronDeviceId: number;
    signatureSessionId: number | null;
    answers: SubmitAnswerRequest[];
}

export interface FormSubmissionAnswerResponse {
    id: number;
    formQuestionId: number;
    questionText: string;
    answerValue: string;
    followUpText: string | null;
}

export interface FormSubmissionBriefResponse {
    id: number;
    formTemplateId: number;
    formTemplateTitle: string;
    templateVersion: number;
    patronDeviceId: number;
    submittedAt: string;
}

export interface FormSubmissionResponse extends FormSubmissionBriefResponse {
    signatureSessionId: number | null;
    answers: FormSubmissionAnswerResponse[];
}
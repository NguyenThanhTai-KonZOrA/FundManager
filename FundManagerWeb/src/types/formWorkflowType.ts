// ─── Workflow ─────────────────────────────────────────────────────────────────
export const StepType = {
    FILLFORM: 1,
    DOCUMENT_AND_SIGNATURE: 2,
} as const;

export type StepType = typeof StepType[keyof typeof StepType];

export interface WorkflowStepResponse {
    id: number;
    stepOrder: number;
    stepType: StepType;
    stepLabel: string;
    formTemplateId: number | null;
    formTemplateTitle: string | null;
    documentTemplateId: number | null;
    documentTemplateTitle: string | null;
}

export interface WorkflowResponse {
    id: number;
    name: string;
    description: string;
    outletId: number;
    outletName: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
    steps: WorkflowStepResponse[];
}

export interface WorkflowStepRequest {
    stepOrder: number;
    stepType: StepType;
    stepLabel: string;
    formTemplateId: number | null;
    documentTemplateId: number | null;
}

export interface CreateWorkflowRequest {
    name: string;
    description: string;
    outletId: number;
    steps: WorkflowStepRequest[];
}

export interface UpdateWorkflowRequest {
    id: number;
    name: string;
    description: string;
    isActive: boolean;
    outletId: number;
    steps: WorkflowStepRequest[];
}
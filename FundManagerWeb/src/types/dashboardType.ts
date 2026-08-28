export interface FormSignedCount {
    documentType: string;
    documentTypeValue: number;
    count: number;
}

export interface WeeklyChart {
    labels: string[];
    thisWeekOnline: number[];
    thisWeekManual: number[];
    lastWeekOnline: number[];
    lastWeekManual: number[];
}

export interface LatestActivity {
    id: number;
    userName: string;
    action: string;
    entityId?: number;
    details?: string;
    ipAddress?: string;
    isSuccess: boolean;
    createdAt: string;
}

export interface DashboardSummaryResponse {
    totalOnlineRegistrations: number;
    totalManualRegistrations: number;
    totalRegistrations: number;
    totalMemberships: number;
    formSignedCounts: FormSignedCount[];
    totalFormSignedPatrons: number;
    totalOtpVerified: number;
    totalVietnamese: number;
    totalForeigners: number;
    weeklyChart: WeeklyChart;
    latestActivities: LatestActivity[];
}
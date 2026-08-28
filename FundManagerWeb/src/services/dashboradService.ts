import type { DashboardSummaryResponse } from "../types/dashboardType";
import { api, unwrapApiEnvelope } from "./commonApiService";

export const dashboardService = {
    getDashboardSummary: async (): Promise<DashboardSummaryResponse> => {
        const res = await api.get("/api/Dashboard/summary");
        return unwrapApiEnvelope(res);
    }
};
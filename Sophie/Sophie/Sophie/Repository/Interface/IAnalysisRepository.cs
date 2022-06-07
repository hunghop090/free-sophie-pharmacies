using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IAnalysisRepository
    {
        Analysis CreateAnalysis(Analysis item);
        Analysis DeleteAnalysis(string analysisId);
        Analysis FindByIdAnalysis(string analysisId);
        Analysis FindByVideoCallIdAnalysis(string videoCallId);
        List<Analysis> ListAnalysis(int pageIndex = 0, int pageSize = 99);
        Analysis RestoreAnalysis(Analysis item);
        long TotalAnalysis();
        Analysis UpdateAnalysis(Analysis item);
    }
}

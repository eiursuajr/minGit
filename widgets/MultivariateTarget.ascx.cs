using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.Widget.Multivariate;
using Ektron.Cms.PageBuilder;
using System.Collections.Generic;
using Ektron.Cms;

public partial class MultivariateTarget : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (Page as PageBuilder == null)
        {
            message.InnerHtml = "Cannot start experiments in a dashboard.";
            return;
        }

        Ektron.Cms.Widget.IWidgetHost host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        host.Title = "Multivariate Target";

        IMultivariateReportModel model = ObjectFactory.GetMultivariateReportModel();

        long pageId = (Page as PageBuilder).Pagedata.pageID;
        List<Combination> combinations = MultivariateCookieHandler.GetCombinations(pageId);

        foreach (Combination combination in combinations)
        {
            if (!combination.IsConverted)
            {
                combination.IsConverted = true;
                MultivariateCookieHandler.SetPageCombination(pageId, combination);
                model.IncrementConversions(combination.ExperimentPageID, new Guid(combination.CombinationGuid));

                IMultivariateExperimentModel expModel = ObjectFactory.GetMultivariateExperimentModel();

                List<MultivariateExperimentData> experiments = expModel.FindByExperimentPageID(combination.ExperimentPageID);
                if (experiments.Count > 0)
                {
                    MultivariateExperimentData experiment = experiments[0];
                    List<MultivariateReportData> reports = model.FindByPageID(experiment.ExperimentPageID);
                    int counter = 0;
                    reports.ForEach(delegate(MultivariateReportData r) { counter += r.Conversions; });
                    if (counter >= experiment.MaxConversions)
                    {
                        reports.Sort(new Comparison<MultivariateReportData>(delegate(MultivariateReportData a, MultivariateReportData b)
                        {
                            if (a == b) return 0;
                            return (a.Conversions > b.Conversions) ? -1 : 1;
                        }));

                        MultivariateReportData topReport = reports[0];
                        reports.RemoveAt(0);

                        model.Update(topReport.ID, topReport.PageID,
                                     topReport.CombinationGuid, topReport.Hits,
                                     topReport.Conversions, false);
                        
                        reports.ForEach(delegate(MultivariateReportData r)
                        {
                            model.Update(r.ID, r.PageID, 
                                         r.CombinationGuid, r.Hits, 
                                         r.Conversions, true);
                        });
                    }
                }
            }
        }

        message.Visible = ((Page as PageBuilder).Status == Mode.Editing);
    }
}

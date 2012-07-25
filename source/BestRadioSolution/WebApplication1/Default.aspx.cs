using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BestRadioNet.Core.Data;
using System.Data.Common;

namespace WebApplication1
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void SampleCode()
        { 
            DataContext context=new DataContext("ConnectionStringKey");

            bool isUpdateOK =context.SaveChange(() =>
            {

                string sql = "Update/Insert/Delete";

                List<DbCommand> list = new List<DbCommand>();
                                
                return list;

            });

            
        }
        
    }
}

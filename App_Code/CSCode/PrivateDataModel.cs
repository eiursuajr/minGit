using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget;
using System.Data.SqlClient;

namespace Ektron.Cms.Workarea.PrivateDataModel
{
    public interface IPrivateDataModel
    {
        byte[] Get(long userId, string key);
        void Set(long userId, string key, byte[] value);
    }

    public class PrivateDataModel : BaseDataModel, IPrivateDataModel
    {
        #region IPrivateDataModel Members

        public void Set(long userId, string key, byte[] value)
        {
            SqlCommand command = CreateDbCommand();
            command.CommandText = "IF NOT EXISTS (SELECT TOP(1) user_id FROM private_data WHERE data_key = @data_key AND user_id = @user_id) INSERT INTO private_data (user_id, data_key, data_value) VALUES (@user_id, @data_key, @data_value) ELSE UPDATE private_data SET data_value = @data_value WHERE data_key = @data_key AND user_id = @user_id;";
            command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
            command.Parameters.Add("@data_key", SqlDbType.NVarChar, 64).Value = key;
            command.Parameters.Add("@data_value", SqlDbType.NText).Value = Convert.ToBase64String(value);
            command.ExecuteNonQuery();
        }

        public byte[] Get(long userId, string key)
        {
            SqlCommand command = CreateDbCommand();
            command.CommandText = "SELECT TOP(1) data_value FROM private_data WHERE user_id = @user_id AND data_key = @data_key;";
            command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
            command.Parameters.Add("@data_key", SqlDbType.NVarChar, 64).Value = key;
            string retval = (string)command.ExecuteScalar();
            return Convert.FromBase64String(retval == null ? "" : retval);
        }

        #endregion
    }
}

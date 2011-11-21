using System;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.Text;

namespace ActiveDirectoryBasicSearch
{
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the Click event of the submitButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void submitButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(nameTextBox.Text))
            {
                DataTable dt = GetModifiedUsers(nameTextBox.Text);

                searchResultsGrid.DataSource = dt;
                searchResultsGrid.DataBind();
            }
        }

        // Method stolen from http://www.dotnettreats.com/tipstricks/adnet.aspx (tweaked slightly)
        /// <summary>
        /// Method used to create an entry to the AD.
        /// Replace the path, username, and password.
        /// </summary>
        /// <returns>DirectoryEntry</returns>
        public static DirectoryEntry GetDirectoryEntry(string path = null)
        {
            string useSecureConnectionString = ConfigurationManager.AppSettings["UseSecureConnection"];
            bool useSecureConnection;
            bool useSecureConnectionStringParsedSuccessfully = bool.TryParse(useSecureConnectionString, out useSecureConnection);

            if (!useSecureConnectionStringParsedSuccessfully)
            {
                throw new ConfigurationErrorsException("UseSecureConnection was not specified in the Web.config as 'true' or 'false'");
            }

            DirectoryEntry de = new DirectoryEntry
            {
                Path = path ?? GetLdapString()
            };

            if (useSecureConnection)
            {
                de.AuthenticationType = AuthenticationTypes.Secure;
            }
            else
            {
                de.Username = ConfigurationManager.AppSettings["LDAPUsername"];
                de.Password = ConfigurationManager.AppSettings["LDAPPassword"];
            }
            
            return de;
        }

        // Method stolen from http://www.dotnettreats.com/tipstricks/adnet.aspx
        /// <summary>
        /// Method that returns a DataTable with a list of users modified from a given date.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public DataTable GetModifiedUsers(string name)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DisplayName");
            dt.Columns.Add("Phone");
            dt.Columns.Add("Office");

            DirectoryEntry de = GetDirectoryEntry();
            DirectorySearcher ds = new DirectorySearcher(de);

            ds.Filter = string.Format("(&(objectCategory=Person)(objectClass=user)(|(givenName={0})(sn={0})))", name);
            ds.SearchScope = SearchScope.Subtree;
            SearchResultCollection results = ds.FindAll();

            foreach (SearchResult result in results)
            {
                DataRow dr = dt.NewRow();
                DirectoryEntry dey = GetDirectoryEntry(result.Path);
                dr["DisplayName"] = dey.Properties["cn"].Value;
                dr["Phone"] = dey.Properties["telephoneNumber"].Value;
                dr["Office"] = dey.Properties["department"].Value;
                dt.Rows.Add(dr);
                dey.Close();
            }

            de.Close();
            return dt;
        }

        /// <summary>
        /// Gets the LDAP string.
        /// </summary>
        /// <returns></returns>
        private static string GetLdapString()
        {
            string ldapServer = ConfigurationManager.AppSettings["LDAPServer"];
            string ldapPath = ConfigurationManager.AppSettings["LDAPPath"];

            if (string.IsNullOrEmpty(ldapServer))
            {
                throw new ConfigurationErrorsException("LDAPServer was not specified in the Web.config");
            }

            if (string.IsNullOrEmpty(ldapPath))
            {
                throw new ConfigurationErrorsException("LDAPPath was not specified in the Web.config");
            }

            return string.Format("LDAP://{0}/{1}", ldapServer, ldapPath);
        }
    }
}
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Repositories.ErmRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.ErmRepositories
{
    public class EmployeeSeparationOutstandingRepository : IEmployeeSeparationOutstandingRepository
    {
        public IConfiguration _config { get; }
        public EmployeeSeparationOutstandingRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Employees Separation Outstandings Write Action Methods

        public async Task<bool> AddAsync(EmployeeSeparationOutstanding e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_spx_outstdns(emp_id, ");
            sb.Append("outstdn_typ_id, item_ds, outstdn_amt, outstdn_curr, ");
            sb.Append("amt_pd, amt_blc, emp_spx_id) VALUES (@emp_id,");
            sb.Append("@outstdn_typ_id, @item_ds, @outstdn_amt, ");
            sb.Append("@outstdn_curr, @amt_pd, @amt_blc, @emp_spx_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                var outstdn_typ_id = cmd.Parameters.Add("@outstdn_typ_id", NpgsqlDbType.Integer);
                var item_ds = cmd.Parameters.Add("@item_ds", NpgsqlDbType.Text);
                var outstdn_amt = cmd.Parameters.Add("@outstdn_amt", NpgsqlDbType.Numeric);
                var outstdn_curr = cmd.Parameters.Add("@outstdn_curr", NpgsqlDbType.Text);
                var amt_pd = cmd.Parameters.Add("@amt_pd", NpgsqlDbType.Numeric);
                var amt_blc = cmd.Parameters.Add("@amt_blc", NpgsqlDbType.Numeric);

                cmd.Prepare();

                emp_id.Value = e.EmployeeId;
                emp_spx_id.Value = e.EmployeeSeparationId;
                outstdn_typ_id.Value = e.TypeId;
                item_ds.Value = e.ItemDescription ?? (object)DBNull.Value;
                outstdn_amt.Value = e.Amount;
                outstdn_curr.Value = e.Currency ?? (object)DBNull.Value;
                amt_pd.Value = e.AmountPaid;
                amt_blc.Value = e.AmountBalance;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditAsync(EmployeeSeparationOutstanding e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_spx_outstdns SET emp_id=@emp_id, ");
            sb.Append("outstdn_typ_id=@outstdn_typ_id, item_ds=@item_ds, ");
            sb.Append("outstdn_amt=@outstdn_amt, outstdn_curr=@outstdn_curr, ");
            sb.Append("amt_pd=@amt_pd, amt_blc=@amt_blc, emp_spx_id=@emp_spx_id ");
            sb.Append("WHERE (outstdn_id=@outstdn_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var outstdn_id = cmd.Parameters.Add("@outstdn_id", NpgsqlDbType.Integer);
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var outstdn_typ_id = cmd.Parameters.Add("@outstdn_typ_id", NpgsqlDbType.Integer);
                var item_ds = cmd.Parameters.Add("@item_ds", NpgsqlDbType.Text);
                var outstdn_amt = cmd.Parameters.Add("@outstdn_amt", NpgsqlDbType.Numeric);
                var outstdn_curr = cmd.Parameters.Add("@outstdn_curr", NpgsqlDbType.Text);
                var amt_pd = cmd.Parameters.Add("@amt_pd", NpgsqlDbType.Numeric);
                var amt_blc = cmd.Parameters.Add("@amt_blc", NpgsqlDbType.Numeric);

                cmd.Prepare();

                outstdn_id.Value = e.Id;
                emp_spx_id.Value = e.EmployeeSeparationId;
                emp_id.Value = e.EmployeeId;
                outstdn_typ_id.Value = e.TypeId;
                item_ds.Value = e.ItemDescription;
                outstdn_amt.Value = e.Amount;
                outstdn_curr.Value = e.Currency;
                amt_pd.Value = e.AmountPaid;
                amt_blc.Value = e.AmountBalance;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id < 1) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_spx_outstdns ");
            sb.Append("WHERE (outstdn_id=@outstdn_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var outstdn_id = cmd.Parameters.Add("@outstdn_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                outstdn_id.Value = id;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int employeeSeparationId, string itemDescription)
        {
            if (employeeSeparationId < 1 || string.IsNullOrWhiteSpace(itemDescription)) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_spx_outstdns ");
            sb.Append("WHERE (emp_spx_id=@emp_spx_id) ");
            sb.Append("AND (item_ds=@item_ds); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                var item_ds = cmd.Parameters.Add("@item_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                emp_spx_id.Value = employeeSeparationId;
                item_ds.Value = itemDescription;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        #region Employees Separation Outstandings Read Action Methods

        public async Task<List<EmployeeSeparationOutstanding>> GetByEmployeeIdAsync(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException(nameof(employeeId), "The required parameter [employeeId] is missing or has in invalid value."); }
            List<EmployeeSeparationOutstanding> employeeSeparationOutstandings = new List<EmployeeSeparationOutstanding>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT o.outstdn_id, o.emp_id, o.outstdn_typ_id, ");
            sb.Append("o.item_ds, o.outstdn_amt, o.outstdn_curr, o.amt_pd, ");
            sb.Append("o.amt_blc, o.emp_spx_id, (SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = o.emp_id) as emp_nm, ");
            sb.Append("CASE WHEN  o.outstdn_typ_id = 0 THEN 'Owed to Staff' ");
            sb.Append("WHEN  o.outstdn_typ_id = 1 THEN 'Owed to Company' ");
            sb.Append("END outstdn_typ_ds, ");

            sb.Append("(SELECT COALESCE(COUNT(pymnt_id),0) ");
            sb.Append("FROM public.erm_spx_pymnts ");
            sb.Append("WHERE outstdn_id = o.outstdn_id) pymnt_no ");

            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.emp_id = @emp_id) ");
            sb.Append("ORDER BY o.item_ds;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                emp_id.Value = employeeId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationOutstandings.Add(new EmployeeSeparationOutstanding
                        {
                            Id = reader["outstdn_id"] == DBNull.Value ? 0 : (int)reader["outstdn_id"],
                            EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString(),
                            TypeId = reader["outstdn_typ_id"] == DBNull.Value ? 0 : (int)(reader["outstdn_typ_id"]),
                            TypeDescription = reader["outstdn_typ_ds"] == DBNull.Value ? string.Empty : reader["outstdn_typ_ds"].ToString(),
                            Amount = reader["outstdn_amt"] == DBNull.Value ? 0.00M : (decimal)(reader["outstdn_amt"]),
                            Currency = reader["outstdn_curr"] == DBNull.Value ? string.Empty : reader["outstdn_curr"].ToString(),
                            AmountPaid = reader["amt_pd"] == DBNull.Value ? 0.00M : (decimal)reader["amt_pd"],
                            AmountBalance = reader["amt_blc"] == DBNull.Value ? 0.00M : (decimal)reader["amt_blc"],
                            ItemDescription = reader["item_ds"] == DBNull.Value ? string.Empty : reader["item_ds"].ToString(),
                            HasPayments = (long)reader["pymnt_no"] == 0 ? false : true,

                    });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationOutstandings;
        }

        public async Task<List<EmployeeSeparationOutstanding>> GetBySeparationIdAsync(int employeeSeparationId)
        {
            if (employeeSeparationId < 1) { throw new ArgumentNullException(nameof(employeeSeparationId), "The required parameter [employeeSeparationId] is missing or has in invalid value."); }
            List<EmployeeSeparationOutstanding> employeeSeparationOutstandings = new List<EmployeeSeparationOutstanding>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT o.outstdn_id, o.emp_id, o.outstdn_typ_id, ");
            sb.Append("o.item_ds, o.outstdn_amt, o.outstdn_curr, o.amt_pd, ");
            sb.Append("o.amt_blc, o.emp_spx_id, (SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = o.emp_id) as emp_nm, ");
            sb.Append("CASE WHEN  o.outstdn_typ_id = 0 THEN 'Owed to Staff' ");
            sb.Append("WHEN  o.outstdn_typ_id = 1 THEN 'Owed to Company' ");
            sb.Append("END outstdn_typ_ds, ");

            sb.Append("(SELECT COALESCE(COUNT(pymnt_id),0) ");
            sb.Append("FROM public.erm_spx_pymnts ");
            sb.Append("WHERE outstdn_id = o.outstdn_id) pymnt_no ");

            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.emp_spx_id = @emp_spx_id) ");
            sb.Append("ORDER BY o.item_ds;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_spx_id.Value = employeeSeparationId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationOutstandings.Add(new EmployeeSeparationOutstanding
                        {
                            Id = reader["outstdn_id"] == DBNull.Value ? 0 : (int)reader["outstdn_id"],
                            EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString(),
                            TypeId = reader["outstdn_typ_id"] == DBNull.Value ? 0 : (int)(reader["outstdn_typ_id"]),
                            TypeDescription = reader["outstdn_typ_ds"] == DBNull.Value ? string.Empty : reader["outstdn_typ_ds"].ToString(),
                            Amount = reader["outstdn_amt"] == DBNull.Value ? 0.00M : (decimal)(reader["outstdn_amt"]),
                            Currency = reader["outstdn_curr"] == DBNull.Value ? string.Empty : reader["outstdn_curr"].ToString(),
                            AmountPaid = reader["amt_pd"] == DBNull.Value ? 0.00M : (decimal)reader["amt_pd"],
                            AmountBalance = reader["amt_blc"] == DBNull.Value ? 0.00M : (decimal)reader["amt_blc"],
                            ItemDescription = reader["item_ds"] == DBNull.Value ? string.Empty : reader["item_ds"].ToString(),
                            HasPayments = (long)reader["pymnt_no"] == 0 ? false : true,
                        });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationOutstandings;
        }

        public async Task<EmployeeSeparationOutstanding> GetByIdAsync(int separationOutstandingId)
        {
            EmployeeSeparationOutstanding e = new EmployeeSeparationOutstanding();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT o.outstdn_id, o.emp_id, o.outstdn_typ_id, ");
            sb.Append("o.item_ds, o.outstdn_amt, o.outstdn_curr, o.amt_pd, ");
            sb.Append("o.amt_blc, emp_spx_id, (SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = o.emp_id) as emp_nm, ");
            sb.Append("CASE WHEN  o.outstdn_typ_id = 0 THEN 'Owed to Staff' ");
            sb.Append("WHEN  o.outstdn_typ_id = 1 THEN 'Owed to Company' ");
            sb.Append("END outstdn_typ_ds, ");

            sb.Append("(SELECT COALESCE(COUNT(pymnt_id),0) ");
            sb.Append("FROM public.erm_spx_pymnts ");
            sb.Append("WHERE outstdn_id = o.outstdn_id) pymnt_no ");

            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.outstdn_id = @outstdn_id); ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var outstdn_id = cmd.Parameters.Add("@outstdn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                outstdn_id.Value = separationOutstandingId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        e.Id = reader["outstdn_id"] == DBNull.Value ? 0 : (int)reader["outstdn_id"];
                        e.EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"];
                        e.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString();
                        e.EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString();
                        e.TypeId = reader["outstdn_typ_id"] == DBNull.Value ? 0 : (int)(reader["outstdn_typ_id"]);
                        e.TypeDescription = reader["outstdn_typ_ds"] == DBNull.Value ? string.Empty : reader["outstdn_typ_ds"].ToString();
                        e.Amount = reader["outstdn_amt"] == DBNull.Value ? 0.00M : (decimal)(reader["outstdn_amt"]);
                        e.Currency = reader["outstdn_curr"] == DBNull.Value ? string.Empty : reader["outstdn_curr"].ToString();
                        e.AmountPaid = reader["amt_pd"] == DBNull.Value ? 0.00M : (decimal)reader["amt_pd"];
                        e.AmountBalance = reader["amt_blc"] == DBNull.Value ? 0.00M : (decimal)reader["amt_blc"];
                        e.ItemDescription = reader["item_ds"] == DBNull.Value ? string.Empty : reader["item_ds"].ToString();
                        e.HasPayments = (long)reader["pymnt_no"] == 0 ? false : true;
                    }
            }
            await conn.CloseAsync();
            return e;
        }

        #endregion

        #region Employees Separation Payments Read Action Methods

        public async Task<List<EmployeeSeparationPayments>> GetPaymentsByEmployeeIdAsync(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException(nameof(employeeId), "The required parameter [employeeId] is missing or has in invalid value."); }
            List<EmployeeSeparationPayments> employeeSeparationPayments = new List<EmployeeSeparationPayments>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.pymnt_id, p.outstdn_id, p.emp_id, p.pymnt_amt, ");
            sb.Append("p.pymnt_curr, p.pymnt_dt, p.pymnt_dtls, p.pymnt_by, ");
            sb.Append("p.rcd_by, p.rcd_dt, p.emp_spx_id, o.item_ds, ");
            sb.Append("o.outstdn_typ_id, (SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = o.emp_id) as emp_nm, ");
            sb.Append("CASE WHEN  o.outstdn_typ_id = 0 THEN 'Owed to Staff' ");
            sb.Append("WHEN  o.outstdn_typ_id = 1 THEN 'Owed to Company' ");
            sb.Append("END outstdn_typ_ds ");
            sb.Append("FROM public.erm_spx_pymnts p ");
            sb.Append("INNER JOIN public.erm_spx_outstdns o ");
            sb.Append("ON p.outstdn_id = o.outstdn_id ");
            sb.Append("WHERE (p.emp_id = @emp_id) ");
            sb.Append("ORDER BY p.pymnt_dt DESC;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                emp_id.Value = employeeId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationPayments.Add(new EmployeeSeparationPayments
                        {
                            Id=reader["pymnt_id"] == DBNull.Value ? 0 : (int)reader["pymnt_id"],
                            OutstandingId = reader["outstdn_id"] == DBNull.Value ? 0 : (int)reader["outstdn_id"],
                            EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString(),
                            ItemTypeDescription = reader["outstdn_typ_ds"] == DBNull.Value ? string.Empty : reader["outstdn_typ_ds"].ToString(),
                            ItemDescription = reader["item_ds"] == DBNull.Value ? string.Empty : reader["item_ds"].ToString(),
                            PaymentAmount = reader["pymnt_amt"] == DBNull.Value ? 0.00M : (decimal)(reader["pymnt_amt"]),
                            Currency = reader["pymnt_curr"] == DBNull.Value ? string.Empty : reader["pymnt_curr"].ToString(),
                            PaymentDate = reader["pymnt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pymnt_dt"],
                            PaymentDetails = reader["pymnt_dtls"] == DBNull.Value ? string.Empty : reader["pymnt_dtls"].ToString(),
                            PaidBy = reader["pymnt_by"] == DBNull.Value ? string.Empty : reader["pymnt_by"].ToString(),
                            EnteredBy = reader["rcd_by"] == DBNull.Value ? string.Empty : reader["rcd_by"].ToString(),
                            EnteredDate = reader["rcd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rcd_dt"],
                        });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationPayments;
        }

        public async Task<List<EmployeeSeparationPayments>> GetPaymentsBySeparationIdAsync(int employeeSeparationId)
        {
            if (employeeSeparationId < 1) { throw new ArgumentNullException(nameof(employeeSeparationId), "The required parameter [employeeSeparationId] is missing or has in invalid value."); }
            List<EmployeeSeparationPayments> employeeSeparationPayments = new List<EmployeeSeparationPayments>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.pymnt_id, p.outstdn_id, p.emp_id, p.pymnt_amt, ");
            sb.Append("p.pymnt_curr, p.pymnt_dt, p.pymnt_dtls, p.pymnt_by, ");
            sb.Append("p.rcd_by, p.rcd_dt, p.emp_spx_id, o.item_ds, ");
            sb.Append("o.outstdn_typ_id, (SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = o.emp_id) as emp_nm, ");
            sb.Append("CASE WHEN  o.outstdn_typ_id = 0 THEN 'Owed to Staff' ");
            sb.Append("WHEN  o.outstdn_typ_id = 1 THEN 'Owed to Company' ");
            sb.Append("END outstdn_typ_ds ");
            sb.Append("FROM public.erm_spx_pymnts p ");
            sb.Append("INNER JOIN public.erm_spx_outstdns o ");
            sb.Append("ON p.outstdn_id = o.outstdn_id ");
            sb.Append("WHERE (p.emp_spx_id = @emp_spx_id) ");
            sb.Append("ORDER BY p.pymnt_dt DESC;");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_spx_id.Value = employeeSeparationId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationPayments.Add(new EmployeeSeparationPayments
                        {
                            Id = reader["pymnt_id"] == DBNull.Value ? 0 : (int)reader["pymnt_id"],
                            OutstandingId = reader["outstdn_id"] == DBNull.Value ? 0 : (int)reader["outstdn_id"],
                            EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString(),
                            ItemTypeDescription = reader["outstdn_typ_ds"] == DBNull.Value ? string.Empty : reader["outstdn_typ_ds"].ToString(),
                            ItemDescription = reader["item_ds"] == DBNull.Value ? string.Empty : reader["item_ds"].ToString(),
                            PaymentAmount = reader["pymnt_amt"] == DBNull.Value ? 0.00M : (decimal)(reader["pymnt_amt"]),
                            Currency = reader["pymnt_curr"] == DBNull.Value ? string.Empty : reader["pymnt_curr"].ToString(),
                            PaymentDate = reader["pymnt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pymnt_dt"],
                            PaymentDetails = reader["pymnt_dtls"] == DBNull.Value ? string.Empty : reader["pymnt_dtls"].ToString(),
                            PaidBy = reader["pymnt_by"] == DBNull.Value ? string.Empty : reader["pymnt_by"].ToString(),
                            EnteredBy = reader["rcd_by"] == DBNull.Value ? string.Empty : reader["rcd_by"].ToString(),
                            EnteredDate = reader["rcd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rcd_dt"],
                        });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationPayments;
        }

        public async Task<List<EmployeeSeparationPayments>> GetPaymentsBySeparationOutstandingIdAsync(int employeeSeparationOutstandingId)
        {
            if (employeeSeparationOutstandingId < 1) { throw new ArgumentNullException(nameof(employeeSeparationOutstandingId), "The required parameter [Employee Outstanding ID] is missing or has in invalid value."); }
            List<EmployeeSeparationPayments> employeeSeparationPayments = new List<EmployeeSeparationPayments>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.pymnt_id, p.outstdn_id, p.emp_id, p.pymnt_amt, ");
            sb.Append("p.pymnt_curr, p.pymnt_dt, p.pymnt_dtls, p.pymnt_by, ");
            sb.Append("p.rcd_by, p.rcd_dt, p.emp_spx_id, o.item_ds, ");
            sb.Append("o.outstdn_typ_id, (SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = o.emp_id) as emp_nm, ");
            sb.Append("CASE WHEN  o.outstdn_typ_id = 0 THEN 'Owed to Staff' ");
            sb.Append("WHEN  o.outstdn_typ_id = 1 THEN 'Owed to Company' ");
            sb.Append("END outstdn_typ_ds ");
            sb.Append("FROM public.erm_spx_pymnts p ");
            sb.Append("INNER JOIN public.erm_spx_outstdns o ");
            sb.Append("ON p.outstdn_id = o.outstdn_id ");
            sb.Append("WHERE (p.outstdn_id = @outstdn_id) ");
            sb.Append("ORDER BY p.pymnt_dt DESC;");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var outstdn_id = cmd.Parameters.Add("@outstdn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                outstdn_id.Value = employeeSeparationOutstandingId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationPayments.Add(new EmployeeSeparationPayments
                        {
                            Id = reader["pymnt_id"] == DBNull.Value ? 0 : (int)reader["pymnt_id"],
                            OutstandingId = reader["outstdn_id"] == DBNull.Value ? 0 : (int)reader["outstdn_id"],
                            EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString(),
                            ItemTypeDescription = reader["outstdn_typ_ds"] == DBNull.Value ? string.Empty : reader["outstdn_typ_ds"].ToString(),
                            ItemDescription = reader["item_ds"] == DBNull.Value ? string.Empty : reader["item_ds"].ToString(),
                            PaymentAmount = reader["pymnt_amt"] == DBNull.Value ? 0.00M : (decimal)(reader["pymnt_amt"]),
                            Currency = reader["pymnt_curr"] == DBNull.Value ? string.Empty : reader["pymnt_curr"].ToString(),
                            PaymentDate = reader["pymnt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pymnt_dt"],
                            PaymentDetails = reader["pymnt_dtls"] == DBNull.Value ? string.Empty : reader["pymnt_dtls"].ToString(),
                            PaidBy = reader["pymnt_by"] == DBNull.Value ? string.Empty : reader["pymnt_by"].ToString(),
                            EnteredBy = reader["rcd_by"] == DBNull.Value ? string.Empty : reader["rcd_by"].ToString(),
                            EnteredDate = reader["rcd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rcd_dt"],
                        });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationPayments;
        }

        public async Task<EmployeeSeparationPayments> GetPaymentByIdAsync(int separationPaymentId)
        {
            EmployeeSeparationPayments e = new EmployeeSeparationPayments();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.pymnt_id, p.outstdn_id, p.emp_id, p.pymnt_amt, ");
            sb.Append("p.pymnt_curr, p.pymnt_dt, p.pymnt_dtls, p.pymnt_by, ");
            sb.Append("p.rcd_by, p.rcd_dt, p.emp_spx_id, o.item_ds, ");
            sb.Append("o.outstdn_typ_id, (SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = o.emp_id) as emp_nm, ");
            sb.Append("CASE WHEN  o.outstdn_typ_id = 0 THEN 'Owed to Staff' ");
            sb.Append("WHEN  o.outstdn_typ_id = 1 THEN 'Owed to Company' ");
            sb.Append("END outstdn_typ_ds ");
            sb.Append("FROM public.erm_spx_pymnts p ");
            sb.Append("INNER JOIN public.erm_spx_outstdns o ");
            sb.Append("ON p.outstdn_id = o.outstdn_id ");
            sb.Append("WHERE (p.pymnt_id = @pymnt_id); ");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var pymnt_id = cmd.Parameters.Add("@pymnt_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                pymnt_id.Value = separationPaymentId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        e.Id = reader["pymnt_id"] == DBNull.Value ? 0 : (int)reader["pymnt_id"];
                        e.OutstandingId = reader["outstdn_id"] == DBNull.Value ? 0 : (int)reader["outstdn_id"];
                        e.EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"];
                        e.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString();
                        e.EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString();
                        e.ItemTypeDescription = reader["outstdn_typ_ds"] == DBNull.Value ? string.Empty : reader["outstdn_typ_ds"].ToString();
                        e.ItemDescription = reader["item_ds"] == DBNull.Value ? string.Empty : reader["item_ds"].ToString();
                        e.PaymentAmount = reader["pymnt_amt"] == DBNull.Value ? 0.00M : (decimal)(reader["pymnt_amt"]);
                        e.Currency = reader["pymnt_curr"] == DBNull.Value ? string.Empty : reader["pymnt_curr"].ToString();
                        e.PaymentDate = reader["pymnt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pymnt_dt"];
                        e.PaymentDetails = reader["pymnt_dtls"] == DBNull.Value ? string.Empty : reader["pymnt_dtls"].ToString();
                        e.PaidBy = reader["pymnt_by"] == DBNull.Value ? string.Empty : reader["pymnt_by"].ToString();
                        e.EnteredBy = reader["rcd_by"] == DBNull.Value ? string.Empty : reader["rcd_by"].ToString();
                        e.EnteredDate = reader["rcd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rcd_dt"];
                    }
            }
            await conn.CloseAsync();
            return e;
        }

        
        //======== Write Action Methods =========//

        public async Task<bool> AddPaymentAsync(EmployeeSeparationPayments p)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_spx_pymnts (outstdn_id, ");
            sb.Append("emp_id, pymnt_amt, pymnt_curr, pymnt_dt, ");
            sb.Append("pymnt_dtls, pymnt_by, rcd_by, rcd_dt, ");
            sb.Append("emp_spx_id) VALUES (@outstdn_id, @emp_id, ");
            sb.Append("@pymnt_amt, @pymnt_curr, @pymnt_dt, ");
            sb.Append("@pymnt_dtls, @pymnt_by, @rcd_by, @rcd_dt, ");
            sb.Append("@emp_spx_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var outstdn_id = cmd.Parameters.Add("@outstdn_id", NpgsqlDbType.Integer);
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                var pymnt_amt = cmd.Parameters.Add("@pymnt_amt", NpgsqlDbType.Numeric);
                var pymnt_curr = cmd.Parameters.Add("@pymnt_curr", NpgsqlDbType.Text);
                var pymnt_dt = cmd.Parameters.Add("@pymnt_dt", NpgsqlDbType.Date);
                var pymnt_dtls = cmd.Parameters.Add("@pymnt_dtls", NpgsqlDbType.Text);
                var pymnt_by = cmd.Parameters.Add("@pymnt_by", NpgsqlDbType.Text);
                var rcd_by = cmd.Parameters.Add("@rcd_by", NpgsqlDbType.Text);
                var rcd_dt = cmd.Parameters.Add("@rcd_dt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                emp_id.Value = p.EmployeeId;
                emp_spx_id.Value = p.EmployeeSeparationId;
                outstdn_id.Value = p.OutstandingId;
                pymnt_amt.Value = p.PaymentAmount;
                pymnt_curr.Value = p.Currency;
                pymnt_dt.Value = p.PaymentDate ?? (object)DBNull.Value;
                pymnt_dtls.Value = p.PaymentDetails ?? (object)DBNull.Value;
                pymnt_by.Value = p.PaidBy ?? (object)DBNull.Value;
                rcd_by.Value = p.EnteredBy ?? (object)DBNull.Value;
                rcd_dt.Value = p.EnteredDate ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditPaymentAsync(EmployeeSeparationPayments p)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.erm_spx_pymnts SET outstdn_id=@outstdn_id, ");
            sb.Append("emp_id=@emp_id, pymnt_amt=@pymnt_amt, pymnt_curr=@pymnt_curr, ");
            sb.Append("pymnt_dt=@pymnt_dt, pymnt_dtls=@pymnt_dtls, pymnt_by=@pymnt_by, ");
            sb.Append("rcd_by=@rcd_by, rcd_dt=@rcd_dt, emp_spx_id=@emp_spx_id ");
            sb.Append("WHERE (pymnt_id=@pymnt_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var pymnt_id = cmd.Parameters.Add("@pymnt_id", NpgsqlDbType.Integer);
                var outstdn_id = cmd.Parameters.Add("@outstdn_id", NpgsqlDbType.Integer);
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                var pymnt_amt = cmd.Parameters.Add("@pymnt_amt", NpgsqlDbType.Numeric);
                var pymnt_curr = cmd.Parameters.Add("@pymnt_curr", NpgsqlDbType.Text);
                var pymnt_dt = cmd.Parameters.Add("@pymnt_dt", NpgsqlDbType.Date);
                var pymnt_dtls = cmd.Parameters.Add("@pymnt_dtls", NpgsqlDbType.Text);
                var pymnt_by = cmd.Parameters.Add("@pymnt_by", NpgsqlDbType.Text);
                var rcd_by = cmd.Parameters.Add("@rcd_by", NpgsqlDbType.Text);
                var rcd_dt = cmd.Parameters.Add("@rcd_dt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                pymnt_id.Value = p.Id;
                outstdn_id.Value = p.OutstandingId;
                emp_id.Value = p.EmployeeId;
                emp_spx_id.Value = p.EmployeeSeparationId;
                pymnt_amt.Value = p.PaymentAmount;
                pymnt_curr.Value = p.Currency;
                pymnt_dt.Value = p.PaymentDate ?? (object)DBNull.Value;
                pymnt_dtls.Value = p.PaymentDetails ?? (object)DBNull.Value;
                pymnt_by.Value = p.PaidBy ?? (object)DBNull.Value;
                rcd_by.Value = p.EnteredBy ?? (object)DBNull.Value;
                rcd_dt.Value = p.EnteredDate ?? (object)DBNull.Value;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            if (id < 1) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_spx_pymnts ");
            sb.Append("WHERE (pymnt_id=@pymnt_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var pymnt_id = cmd.Parameters.Add("@pymnt_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                pymnt_id.Value = id;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        #region Employee Separation Outstanding Items Action Methods
        public async Task<bool> AddSeparationOutstandingItemAsync(SeparationOutstandingItem item)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_spx_fnitms(spx_fnitms_ds) ");
            sb.Append("VALUES (@spx_fnitms_ds);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_fnitms_ds = cmd.Parameters.Add("@spx_fnitms_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                spx_fnitms_ds.Value = item.Description;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditSeparationOutstandingItemAsync(SeparationOutstandingItem item)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_spx_fnitms SET spx_fnitms_ds=@spx_fnitms_ds ");
            sb.Append("WHERE (spx_fnitms_id=@spx_fnitms_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_fnitms_id = cmd.Parameters.Add("@spx_fnitms_id", NpgsqlDbType.Integer);
                var spx_fnitms_ds = cmd.Parameters.Add("@spx_fnitms_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                spx_fnitms_id.Value = item.Id;
                spx_fnitms_ds.Value = item.Description;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteSeparationOutstandingItemAsync(int id)
        {
            if (id < 1) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_spx_fnitms ");
            sb.Append("WHERE (spx_fnitms_id=@spx_fnitms_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_fnitms_id = cmd.Parameters.Add("@spx_fnitms_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                spx_fnitms_id.Value = id;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<SeparationOutstandingItem> GetSeparationOutstandingItemByIdAsync(int id)
        {
            SeparationOutstandingItem e = new SeparationOutstandingItem();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_fnitms_id, spx_fnitms_ds FROM public.erm_spx_fnitms ");
            sb.Append("WHERE (spx_fnitms_id=@spx_fnitms_id);");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var spx_fnitms_id = cmd.Parameters.Add("@spx_fnitms_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                spx_fnitms_id.Value = id;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        e.Id = reader["spx_fnitms_id"] == DBNull.Value ? 0 : (int)reader["spx_fnitms_id"];
                        e.Description = reader["spx_fnitms_ds"] == DBNull.Value ? string.Empty : reader["spx_fnitms_ds"].ToString();
                    }
            }
            await conn.CloseAsync();
            return e;
        }

        public async Task<List<SeparationOutstandingItem>> GetSeparationOutstandingItemsAsync()
        {
            List<SeparationOutstandingItem> itemList = new List<SeparationOutstandingItem>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_fnitms_id, spx_fnitms_ds ");
            sb.Append("FROM public.erm_spx_fnitms ");
            sb.Append("ORDER BY spx_fnitms_ds;");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        itemList.Add(
                          new SeparationOutstandingItem
                          {
                              Id = reader["spx_fnitms_id"] == DBNull.Value ? 0 : (int)reader["spx_fnitms_id"],
                              Description = reader["spx_fnitms_ds"] == DBNull.Value ? string.Empty : reader["spx_fnitms_ds"].ToString()
                          });
                    }
            }
            await conn.CloseAsync();
            return itemList;
        }

        #endregion


    }
}

﻿using System;
using De.Roslan.LexofficeDotnet.Enums;
using De.Roslan.LexofficeDotnet.Models;
using De.Roslan.LexofficeDotnet.Models.VoucherList;

namespace De.Roslan.LexofficeDotnet.Endpoints {
    public class VoucherListHandler : EndPointHandler, IVoucherListEndPoint
    {


        internal VoucherListHandler(RestClient client) : base(client) {}



        /// <summary>
        /// Returns the VoucherList with the given filters and page settings.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="archived"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public LexOfficeResponse<ResourcePage<VoucherListEntry>> GetVoucherList(VoucherType type, VoucherStatus status, VoucherListArchived archived, int page = 0, int pageSize = 25) {

            var result = PrepareVoucherListString(type, status, archived, page, pageSize);
            var response = client.SendGetRequest<ResourcePage<VoucherListEntry>>(result);
            return new LexOfficeResponse<ResourcePage<VoucherListEntry>>(response);

        }


        
        /// <summary>
        /// Returns the VoucherList with the given filters, page settings and sort settings.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="archived"></param>
        /// <param name="desc"></param>
        /// <param name="sorter"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public LexOfficeResponse<ResourcePage<VoucherListEntry>> GetVoucherListSorted(VoucherType type, VoucherStatus status, VoucherListArchived archived, bool desc, VoucherListSorter sorter, int page = 0, int pageSize = 25) {
            var result = PrepareVoucherListString(type, status, archived, page, pageSize);



            // Parse Sorter
            // Parse Status
            string strSorter = "sort=";
            foreach (VoucherListSorter e in Enum.GetValues(typeof(VoucherListSorter))) {
                if (sorter.HasFlag(e)) {
                    strSorter += e.ToString().ToLower() + ",";
                }
            }
            // TODO: Status "Overdue" cannot be filtered with other status filters
            strSorter = strSorter.Substring(0, strSorter.Length - 1);


            if (desc) {
                strSorter += ",DESC";
            } else {
                strSorter += ",ASC";
            }

            result += $"&{strSorter}&";
            var response = client.SendGetRequest<ResourcePage<VoucherListEntry>>(result);
            return new LexOfficeResponse<ResourcePage<VoucherListEntry>>(response);
        }



        /// <summary>
        /// Prepare and populate the REST-resource string that should be send
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status">Filters the vouchers by their status. Bit flags can be used here. Warning: Status "Overdue" can not be used together with other status.</param>
        /// <param name="archived"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private string PrepareVoucherListString(VoucherType type, VoucherStatus status, VoucherListArchived archived, int page, int pageSize) {
            string resource = "/voucherlist?";



            // Parse Type
            string vouchertype = "voucherType=";
            foreach (VoucherType e in Enum.GetValues(typeof(VoucherType))) {
                if (type.HasFlag(e)) {
                    vouchertype += e.ToString().ToLower() + ",";
                }
            }
            vouchertype = vouchertype.Substring(0, vouchertype.Length - 1);



            // Parse Status
            string voucherstatus = "voucherStatus=";
            foreach (VoucherStatus e in Enum.GetValues(typeof(VoucherStatus))) {
                if (status.HasFlag(e)) {
                    voucherstatus += e.ToString().ToLower() + ",";
                }
            }
            // TODO: Status "Overdue" cannot be filtered with other status filters
            voucherstatus = voucherstatus.Substring(0, voucherstatus.Length - 1);



            // Parse Archived
            string strArchived = "";
            if (archived != VoucherListArchived.Both) {
                switch (archived) {
                    case VoucherListArchived.Archived:
                        strArchived = "archived=true";
                        break;
                    case VoucherListArchived.NonArchived:
                        strArchived = "archived=false";
                        break;
                }
            }



            // Parse PageSize
            string strPageSize = "";
            if (pageSize != 25) {
                if (pageSize > 250) {
                    pageSize = 250;
                }

                if (pageSize < 1) {
                    pageSize = 1;
                }
                strPageSize = $"size={pageSize}";
            }



            // Parse Page
            string strPage = "";
            if (page > 0) {
                strPage = $"page={page}";
            }

            string result = $"{resource}{vouchertype}&{voucherstatus}&{strArchived}&{strPageSize}&{strPage}";

            return result;
        }
    }
}
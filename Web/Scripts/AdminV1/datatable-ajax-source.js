//NamMV 09/12/2014
var DatatableAjaxSource = function () {
    var tableOptions;
    var dataTable;
    var table;
    var tableContainer;
    var tableWrapper;
    var tableInitialized = false;
    var ajaxParams = {};
    var the;

    var countSelectedRecords = function () {
        var selected = $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
        var text = tableOptions.dataTable.language.adminversiononeGroupActions;
        if (selected > 0) {
            $('.table-group-actions > span', tableWrapper).text(text.replace("_TOTAL_", selected));
        } else {
            $('.table-group-actions > span', tableWrapper).text("");
        }
    };

    return {
        init: function (options) {

            if (!$().dataTable) {
                return;
            }

            the = this;
            options = $.extend(true, {
                src: "",
                filterApplyAction: "filter",
                filterCancelAction: "filter_cancel",
                resetGroupActionInputOnSuccess: true,
                loadingMessage: 'Đang tải dữ liệu...',
                dataTable: {
                    //"dom": "<'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>", 
                    //"dom": "<'row'<'col-md-8 col-sm-12'<'table-group-actions'>><'col-md-4 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-12 col-sm-12'pli>>",
                    //"dom": "<'row'<'col-md-12'<'table-group-actions'>>r><'table-scrollable't><'row'<'col-md-8 col-sm-12'pli><'col-md-4 col-sm-12'>>",                     
                    "dom": "<'row'<'col-md-6 col-sm-12'<'table-group-actions'>><'col-md-6 col-sm-12 display-inline'<'display-inline pull-right'f><'table-group-actions-2 display-inline pull-right'>>r><'table-scrollable't><'row'<'col-md-12 col-sm-12'pli>>",
                    "lengthMenu": [
                            [10, 20, 50, 100, 150, -1],
                            [10, 20, 50, 100, 150, "Tất cả"]//Ô chọn hiển thị số bản ghi mỗi trang
                    ],
                    "pageLength": 10, // mặc định số bản ghi mỗi trang
                    "language": {
                        "adminversiononeGroupActions": " _TOTAL_ bản ghi đã chọn",
                        "adminversiononeAjaxRequestGeneralError": "Không lấy được dữ liệu, vui lòng thử lại!",
                        "search": "Tìm kiếm: ",
                        "lengthMenu": "<span class='seperator'>|</span>Hiển thị _MENU_ bản ghi mỗi trang",
                        "info": "<span class='seperator'>|</span>Tổng số _TOTAL_ bản ghi",
                        "infoFiltered": "(tìm trong tổng số _MAX_ bản ghi)",
                        "infoEmpty": " Không tìm thấy bản ghi nào",
                        "emptyTable": " Không có dữ liệu",
                        "zeroRecords": " Không tìm thấy dữ liệu",
                        "paginate": {
                            "previous": "Trang trước",
                            "next": "Trang sau",
                            "last": "Trang cuối",
                            "first": "Trang đầu",
                            "page": "Trang",
                            "pageOf": " trong tổng số"
                        }
                    },

                    "orderCellsTop": true,
                    "columnDefs": [{
                        'orderable': false,
                        'targets': [0, -1]
                    }],

                    "pagingType": "bootstrap_extended", // pagination type(bootstrap_full_number or bootstrap_extended)
                    "autoWidth": false,
                    "processing": false,
                    "serverSide": true,
                    //"deferRender": true,//Tăng tốc độ tải dữ liệu
                    "ajax": {
                        "url": "",
                        "type": "GET", //NamMV đổi thành GET
                        "timeout": 20000,
                        "data": function (data) { // truyền tham số trước khi submit                            
                            $.each(ajaxParams, function (key, value) {
                                data[key] = value;
                            });
                            AdminVersionOne.blockUI({
                                message: tableOptions.loadingMessage,
                                target: tableContainer,
                                overlayColor: 'none',
                                cenrerY: true,
                                boxed: true
                            });
                        },
                        "dataSrc": function (res) {
                            if (res.customActionMessage) {
                                AdminVersionOne.alert({
                                    type: (res.customActionStatus == 'OK' ? 'success' : 'danger'),
                                    icon: (res.customActionStatus == 'OK' ? 'check' : 'warning'),
                                    message: res.customActionMessage,
                                    container: tableWrapper,
                                    place: 'prepend'
                                });
                            }

                            if ($('.group-checkable', table).size() === 1) {
                                $('.group-checkable', table).attr("checked", false);
                                $.uniform.update($('.group-checkable', table));
                            }

                            if (tableOptions.onSuccess) {
                                tableOptions.onSuccess.call(undefined, the);
                            }

                            AdminVersionOne.unblockUI(tableContainer);
                            //alert(res.data)
                            //alert(res.recordsTotal)
                            return res.data;
                        },
                        "error": function () {
                            if (tableOptions.onError) {
                                tableOptions.onError.call(undefined, the);
                            }

                            AdminVersionOne.alert({
                                type: 'danger',
                                icon: 'warning',
                                message: tableOptions.dataTable.language.adminversiononeAjaxRequestGeneralError,
                                container: tableWrapper,
                                place: 'prepend'
                            });

                            AdminVersionOne.unblockUI(tableContainer);
                        }
                    },

                    "drawCallback": function (oSettings) {
                        if (tableInitialized === false) {
                            tableInitialized = true;
                            table.show();
                        }
                        AdminVersionOne.initUniform($('input[type="checkbox"]', table));
                        countSelectedRecords();
                    }
                }
            }, options);

            tableOptions = options;

            table = $(options.src);
            tableContainer = table.parents(".table-container");

            var tmp = $.fn.dataTableExt.oStdClasses;

            $.fn.dataTableExt.oStdClasses.sWrapper = $.fn.dataTableExt.oStdClasses.sWrapper + " dataTables_extended_wrapper";
            $.fn.dataTableExt.oStdClasses.sFilterInput = "form-control input-small input-sm input-inline";
            $.fn.dataTableExt.oStdClasses.sLengthSelect = "form-control input-xsmall input-sm input-inline";

            dataTable = table.DataTable(options.dataTable);

            $.fn.dataTableExt.oStdClasses.sWrapper = tmp.sWrapper;
            $.fn.dataTableExt.oStdClasses.sFilterInput = tmp.sFilterInput;
            $.fn.dataTableExt.oStdClasses.sLengthSelect = tmp.sLengthSelect;

            tableWrapper = table.parents('.dataTables_wrapper');

            if ($('.table-actions-wrapper', tableContainer).size() === 1) {
                $('.table-group-actions', tableWrapper).html($('.table-actions-wrapper', tableContainer).html());
                $('.table-actions-wrapper', tableContainer).remove();
            }
            // build table group actions panel
            if ($('.table-actions-2-wrapper', tableContainer).size() === 1) {
                $('.table-group-actions-2', tableWrapper).html($('.table-actions-2-wrapper', tableContainer).html()); // place the panel inside the wrapper
                $('.table-actions-2-wrapper', tableContainer).remove(); // remove the template container
            }
            $('.group-checkable', table).change(function () {
                var set = $('tbody > tr > td:nth-child(1) input[type="checkbox"]', table);
                var checked = $(this).is(":checked");
                $(set).each(function () {
                    $(this).attr("checked", checked);
                    if (checked) {
                        $(this).parents('tr').addClass("active");
                    } else {
                        $(this).parents('tr').removeClass("active");
                    }
                });
                $.uniform.update(set);
                countSelectedRecords();
            });
            table.on('change', 'tbody tr .checkboxes', function () {
                $(this).parents('tr').toggleClass("active");
            });
            table.on('change', 'tbody > tr > td:nth-child(1) input[type="checkbox"]', function () {
                countSelectedRecords();
            });

            table.on('search.dt', function () {
                if ($('select.object-filter'))
                    the.setAjaxParam('objectStatus', $('select.object-filter').val());
            });
            table.on('order.dt', function () {
                if ($('select.object-filter'))
                    the.setAjaxParam('objectStatus', $('select.object-filter').val());
            });
            table.on('page.dt', function () {
                if ($('select.object-filter'))
                    the.setAjaxParam('objectStatus', $('select.object-filter').val());
            });


            table.on('search.dt', function () {
                if ($('.object-filter-tu'))
                    the.setAjaxParam('objectGiaTu', $('.object-filter-tu').val());
            });

            table.on('search.dt', function () {
                if ($('.object-filter-den'))
                    the.setAjaxParam('objectGiaDen', $('.object-filter-den').val());
            });

            table.on('search.dt', function () {
                if ($('.object-filter-sodienthoai'))
                    the.setAjaxParam('objectSoDienThoai', $('.object-filter-sodienthoai').val());
            });

            table.on('search.dt', function () {
                if ($('.object-filter-quan'))
                    the.setAjaxParam('objectQuan', $('.object-filter-quan').val());
            });

            table.on('search.dt', function () {
                if ($('.object-filter-duong'))
                    the.setAjaxParam('objectDuong', $('.object-filter-duong').val());
            });

            table.on('search.dt', function () {
                if ($('.object-filter-matien'))
                    the.setAjaxParam('objectMatTien', $('.object-filter-mattien').val());
            });

            table.on('search.dt', function () {
                if ($('.object-filter-dientich'))
                    the.setAjaxParam('objectDienTich', $('.object-filter-dientich').val());
            });

            table.on('search.dt', function () {
                if ($('.object-filter-ghichu'))
                    the.setAjaxParam('objectGhiChu', $('.object-filter-ghichu').val());
            });

        },

        getSelectedRowsCount: function () {
            return $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
        },

        getSelectedRows: function () {
            var rows = [];
            $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                rows.push($(this).val());
            });

            return rows;
        },

        setAjaxParam: function (name, value) {
            ajaxParams[name] = value;
        },

        addAjaxParam: function (name, value) {
            if (!ajaxParams[name]) {
                ajaxParams[name] = [];
            }

            skip = false;
            for (var i = 0; i < (ajaxParams[name]).length; i++) {//Kiểm tra trùng
                if (ajaxParams[name][i] === value) {
                    skip = true;
                }
            }

            if (skip === false) {
                ajaxParams[name].push(value);
            }
        },

        clearAjaxParams: function (name, value) {
            ajaxParams = {};
        },

        getDataTable: function () {
            return dataTable;
        },

        getTableWrapper: function () {
            return tableWrapper;
        },

        gettableContainer: function () {
            return tableContainer;
        },

        getTable: function () {
            return table;
        }

    };

};
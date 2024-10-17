const getSelectedBrands = () => {
    let brands = $('input[name="filter-brand"]:checked').map(function () {
        return $(this).val() || null;
    }).toArray();

    brands = Array.from(new Set(brands));

    return brands;
}

const getSelectedCategories = () => {
    let categories = $('input[name="filter-category"]:checked').map(function () {
        return $(this).val() || null;
    }).toArray() || null;

    categories = Array.from(new Set(categories));

    return categories;
}

const getSelectedSatus = () => {
    const status = $('input[name="filter-status"]:checked').val() || null;
    const status_from_all = $('input[name="stt_filter-status"]:checked').val() || null;

    return status || status_from_all;
}

const getPriceRange = () => {
    const min_price = $('#min-price').val() || null;
    const max_price = $('#max-price').val() || null;

    return {
        min_price,
        max_price
    };
}

const getSelectedWarehouses = () => {
    const warehouse = $('.select-warehouse').find('.page-dropdown-btn').data('selected') || null;

    return warehouse;
}

const getSelectedSortValue = () => {
    const sort = $('.sort-selection').find('.page-dropdown-btn').data('selected') || null;

    return sort;
}

const getSelectedViewType = () => {
    const view = $('.view-selection').find('.page-dropdown-btn').data('selected') || null;

    return view;
}

const updateFilterItems = (brands, categories, status, price_range) => {
    if (!brands.length && !categories.length && !status && !price_range.length) {
        $('.filter-btn').removeClass('active');
        return;
    }

    $('input[name="filter-brand"]').prop('checked', false);
    $('input[name="filter-category"]').prop('checked', false);
    $('#filter-status-all, #stt_filter-status-all').prop('checked', true);

    if (brands.length) {
        $('.filter-all .filter-btn, .filter-brands .filter-btn').addClass('active');

        brands.map((brand) => {
            $(`input[name="filter-brand"][value="${brand}"]`).prop('checked', true);
        });
    } else {
        $('.filter-brands .filter-btn').removeClass('active');
    }

    if (categories.length) {
        $('.filter-all .filter-btn, .filter-categories .filter-btn').addClass('active');

        categories.map((category) => {
            $(`input[name="filter-category"][value="${category}"]`).prop('checked', true);
        });
    } else {
        $('.filter-categories .filter-btn').removeClass('active');
    }

    if (status) {
        $('.filter-all .filter-btn, .filter-status .filter-btn').addClass('active');
        $(`input[name="filter-status"][value="${status}"], input[name="stt_filter-status"][value="${status}"]`).prop('checked', true);
    } else {
        $('.filter-status .filter-btn').removeClass('active');
    }


    $('#min-price').val(price_range?.min_price || 0);
    $('#max-price').val(price_range?.max_price || 1000000);
}

const updateActionButtons = (products) => {
    const deleted = products.filter(product => product.isDeleted === true).length;
    const activated = products.filter(product => product.isActive === true).length;
    const inactive = products.filter(product => product.isActive === false && product.isDeleted == false).length;
    const total = products.length;

    if (deleted === total) {
        $('.delete-selected-products').addClass('d-none');
        $('.restore-selected-products').removeClass('d-none');
    } else {
        $('.delete-selected-products').removeClass('d-none');
        $('.restore-selected-products').addClass('d-none');
    }

    if (activated === total || deleted === total) {
        $('.activate-selected-products').addClass('d-none');
    } else {
        $('.activate-selected-products').removeClass('d-none');
    }

    if (inactive === total || deleted === total) {
        $('.deactivate-selected-products').addClass('d-none');
    } else {
        $('.deactivate-selected-products').removeClass('d-none');
    }
}

const renderProducts = (products, currentPage) => {
    const view_type = $('.product-list').data('view');

    let index = (currentPage - 1) * 40;

    $('.product-list').empty();

    products.forEach((product) => {
        index++;
        const imageUrl = product.productImages[0]?.imageSrc || '/admin/images/no-image.jpg';
        const total_qty = product.warehouseProducts.reduce((accumulator, item) => {
            return accumulator + item.quantity;
        }, 0);

        let status_badge = '<span class="page-badge badge-success">Còn hàng</span>';

        if (product.isDeleted === true) {
            status_badge = '<span class="page-badge badge-error">Đã xóa</span>';
        }
        else if (!product.isActive == true) {
            status_badge = '<span class="page-badge badge-warning">Chưa kích hoạt</span>';
        }
        else if (total_qty <= 0) {
            status_badge = '<span class="page-badge badge-error">Hết hàng</span>';
        }
        else if (total_qty <= 5) {
            status_badge = '<span class="page-badge badge-warning">Sắp hết hàng</span>';
        }

        $('.product-list').append(`
            ${view_type === 'view-grid'
            ? `
                <div class="product-card">
                    <div class="product-card__image lazy-loading">
                        <img lazy-src="${imageUrl}" alt="">
                    </div>

                    <div class="product-card__info">
                        <p class="product-card__name text-overflow-2 text-product-name mb-2">${product.productName}</p>
                        <p class="product-card__price text-price">${product.price.toLocaleString('vi-VN')}đ</p>
                    </div>

                    <div class="product-card__actions d-flex gap-1 align-items-center justify-content-end">
                        <div class="flex-grow-1">
                            ${status_badge}
                        </div>

                        <a href="/admin/products/1/${product.productId}" class="page-btn-small btn-lightblue">
                            <i class="fa-regular fa-pen-to-square"></i>
                        </a>

                        ${product.isDeleted !== true
                            ? ` <button class="delete-product page-btn-small btn-red" data-product="${product.productId}">
                                                        <i class="fa-solid fa-trash-can"></i>
                                                    </button>`
                            : ''
                        }
                    </div>
                </div>
            `

            : `
                <tr data-item="${product.productId}">
                    <td>
                        <input type="checkbox" name="page-table-checkbox" value="${product.productId}" />
                    </td>
                    <td>
                        ${index}
                    </td>
                    <td>
                        <div>
                            <img lazy-src="${imageUrl}" alt="" style="width: 2rem">
                        </div>
                    </td>
                    <td>
                        <span class="text-overflow-1">${product.productName}</span>
                    </td>
                    <td>
                        ${product.originalPrice.toLocaleString('vi-VN')}đ
                    </td>
                    <td>
                        ${product.price.toLocaleString('vi-VN')}đ
                    </td>
                    <td>
                        ${status_badge}
                    </td>
                    <td>
                        ${total_qty}
                    </td>
                    <td>
                        <div class="d-flex gap-1 align-items-center justify-content-end">
                            <a class="page-table-btn btn-lightblue" href="/admin/products/1/${product.productId}">
                                <i class="fa-regular fa-pen-to-square"></i>
                            </a>
                            ${product.isDeleted !== true
                            ? ` <button class="page-table-btn btn-red delete-product" data-product="${product.productId}">
                                    <i class="fa-solid fa-trash-can"></i>
                                </button>`
                            : ''
                            }
                        </div>
                    </td>
                </tr>
            `
            }
        `);
    })

    lazyLoading();
}

const loadProducts = (page) => {
    $('.search-products input').val(null);
    $('#page-table-checkbox-all').prop('checked', false);
    $('.hidden-action').removeClass('show');

    const brands = getSelectedBrands() || [];
    const categories = getSelectedCategories() || [];
    const status = getSelectedSatus() || null;
    const warehouse = getSelectedWarehouses() || null;
    const price_range = getPriceRange() || null;

    const sort = getSelectedSortValue() || null;

    const price_range_str = price_range.min_price && price_range.max_price ? `${price_range.min_price},${price_range.max_price}` : null;

    $('.filter-contents').removeClass('show');

    const data = {
        brands: brands.join(','),
        categories: categories.join(','),
        status: status,
        warehouse_id: warehouse,
        price_range: price_range_str,
        page: page,
        sort: sort
    };

    updateUrlPath(`/admin/products`);
    updateParams(data);
    updateFilterItems(brands, categories, status, price_range);

    showWebLoader();
    $.ajax({
        type: 'GET',
        url: `/api/admin/products`,
        data: data,
        success: (response) => {
            hideWebLoader(500);            

            $('.product-list').empty();

            if (!response.data.products.length) {
                $('.empty-message').removeClass('d-none');
                loadPagination(1, 1);
                return;
            }

            $('.empty-message').addClass('d-none');

            updateActionButtons(response.data.products);
            renderProducts(response.data.products, response.data.currentPage);
            loadPagination(response.data.totalPages, response.data.currentPage);
        },
        error: () => {
            hideWebLoader(0);
            showDialog('error', 'Không thể lấy danh sách sản phẩm', null);
            loadPagination(1, 1);
        }
    })
};

$(document).ready(() => {
    const urlParams = new URLSearchParams(window.location.search);

    const brands = urlParams.get('brands')?.split(',').filter(brand => brand.trim() !== '') || [];
    const categories = urlParams.get('categories')?.split(',').filter(category => category.trim() !== '') || [];
    const status = urlParams.get('status') || null;
    const warehouse = urlParams.get('warehouse_id') || null;
    const price_range = urlParams.get('price_range')?.split(',').filter(price => price.trim() !== '') || [];

    const sort = urlParams.get('sort') || null;
    const view_type = urlParams.get('view_type') || null;


    activeDropdown('.select-warehouse', warehouse);
    activeDropdown('.sort-selection', sort);
    activeDropdown('.view-selection', view_type);

    updateFilterItems(brands, categories, status, { min_price: price_range?.[0], max_price: price_range?.[1] });
})

$('.select-warehouse .page-dropdown-item, .sort-selection .page-dropdown-item').click(() => {
    loadProducts(1);
})

$('.view-selection .page-dropdown-item').click(() => {
    const view_type = getSelectedViewType() || null;

    updateParams({
        view_type: view_type
    });

    window.location.reload();
})


$('.filter-item').not('.filter-all').find('.filter-submit-action').click(() => {
    $('.filter-all').find('input').prop('checked', false);
    loadProducts(1);
});

$('.filter-all .filter-submit-action').click(() => {
    $('.filter-categories').find('input[name="filter-category"]').prop('checked', false);
    $('.filter-brands').find('input[name="filter-brand"]').prop('checked', false);
    $('.filter-status').find('input[name="filter-status"]').prop('checked', false);

    loadProducts(1);
})

$('.filter-item').not('.filter-all').find('.filter-cancel-action').click(function () {
    const filter_contents = $(this).closest('.filter-contents');
    filter_contents.find('#filter-status-all').prop('checked', true);
    filter_contents.find('input').prop('checked', false);

    if (filter_contents.find('input[name="filter-category"]').length) {
        $('.filter-all').find('input[name="filter-category"]').prop('checked', false);
    }

    if (filter_contents.find('input[name="filter-brand"]').length) {
        $('.filter-all').find('input[name="filter-brand"]').prop('checked', false);
    }

    if (filter_contents.find('input[name="filter-status"]').length) {
        $('#stt_filter-status-all').prop('checked', true);
    }

    loadProducts(1);
})

$('.filter-all .filter-cancel-action').click(() => {
    $('input[name="filter-category"]').prop('checked', false);
    $('input[name="filter-brand"]').prop('checked', false);
    $('#filter-status-all').prop('checked', true);
    $('#stt_filter-status-all').prop('checked', true);

    loadProducts(1);
})

$(document).on('click', '.pagination-item:not(.current)', function () {
    const page = $(this).data('page') || '1';
    const current_page = parseInt($('.pagination-item.current').data('page') || '1');

    if (page === 'next') {
        loadProducts(current_page + 1);
    } else if (page === 'previous') {
        loadProducts(current_page - 1);
    } else {
        loadProducts(page);
    }
})



const getCheckedProducts = () => {
    return $('input[name="page-table-checkbox"]:checked').map(function () {
        return $(this).val();
    }).toArray();
}


$(document).on('click', '.delete-product', function () {
    const id = $(this).data('product');

    showConfirmDialog('Xóa sản phẩm này?', 'Hành động này không thể hoàn tác', () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/delete/1/${id}`,
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialog('info', 'Xóa thành công', 'Đã xóa sản phẩm này');
                    loadProducts(parseInt($('.pagination-item.current').data('page') || '1'));
                } else {
                    showDialog('error', 'Không thể xóa sản phẩm', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể xóa sản phẩm', null);
            }
        })
    })
}) 

$('.delete-selected-products').click(() => {
    const checked_products = getCheckedProducts();

    showConfirmDialog('Xóa các sản phẩm đã chọn?', 'Hành động này không thể hoàn tác', () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/delete/range`,
            contentType: 'application/json',
            data: JSON.stringify(checked_products),
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialog('info', 'Xóa thành công', 'Đã xóa các sản phẩm đã chọn');
                    loadProducts(parseInt($('.pagination-item.current').data('page') || '1'));
                } else {
                    showDialog('error', 'Không thể xóa sản phẩm', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể xóa sản phẩm', null);
            }
        })
    })
})

$('.restore-selected-products').click(() => {
    const checked_products = getCheckedProducts();

    showConfirmDialog('Khôi phục các sản phẩm đã chọn?', null, () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/restore/range`,
            contentType: 'application/json',
            data: JSON.stringify(checked_products),
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialog('info', 'Khôi phục thành công', 'Đã khôi phục các sản phẩm đã chọn');
                    loadProducts(parseInt($('.pagination-item.current').data('page') || '1'));
                } else {
                    showDialog('error', 'Không thể khôi phục các sản phẩm này', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể khôi phục các sản phẩm này', null);
            }
        })
    })
})

$('.activate-selected-products').click(() => {
    const checked_products = getCheckedProducts();

    showConfirmDialog('Kích hoạt các sản phẩm đã chọn?', null, () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/activate/range`,
            contentType: 'application/json',
            data: JSON.stringify(checked_products),
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialog('info', 'Kích hoạt thành công', 'Đã kích hoạt các sản phẩm đã chọn');
                    loadProducts(parseInt($('.pagination-item.current').data('page') || '1'));
                } else {
                    showDialog('error', 'Không thể kích hoạt các sản phẩm này', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể kích hoạt các sản phẩm này', null);
            }
        })
    })
})

$('.deactivate-selected-products').click(() => {
    const checked_products = getCheckedProducts();

    showConfirmDialog('Hủy kích hoạt các sản phẩm đã chọn?', null, () => {
        showWebLoader();
        $.ajax({
            type: 'PATCH',
            url: `/api/admin/products/deactivate/range`,
            contentType: 'application/json',
            data: JSON.stringify(checked_products),
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialog('info', 'Hủy kích hoạt thành công', 'Đã hủy kích hoạt các sản phẩm đã chọn');
                    loadProducts(parseInt($('.pagination-item.current').data('page') || '1'));
                } else {
                    showDialog('error', 'Không thể hủy kích hoạt các sản phẩm này', null);
                }
            },
            error: () => {
                hideWebLoader(0);
                showDialog('error', 'Không thể hủy kích hoạt các sản phẩm này', null);
            }
        })
    })
})

$('.search-products').submit(function (e) {
    e.preventDefault();

    $('#page-table-checkbox-all').prop('checked', false);
    $('.filter-contents').find('input').prop('checked', false);
    $('.hidden-action').removeClass('show');

    updateFilterItems([], [], null, []);

    const query = $(this).find('input').val();
    const warehouse = getSelectedWarehouses() || null;

    updateUrlPath(`/admin/products/search/${query}`);
    updateParams({
        warehouse_id: warehouse,
    });

    showWebLoader();
    $.ajax({
        type: 'GET',
        url: `/api/admin/products/search/${query}`,
        data: {
            warehouse_id: warehouse
        },
        success: (response) => {
            hideWebLoader(300);
            $('.product-list').empty();
            if (!response.data.products.length) {
                $('.empty-message').removeClass('d-none');
                loadPagination(1, 1);
                return;
            }

            $('.empty-message').addClass('d-none');
            renderProducts(response.data.products, 1);
            loadPagination(1, 1);
        },
        error: () => {
            showErrorDialog();
            hideWebLoader(0);
        }
    })
})


let typingTimeout;
$('.search-products #search').keyup(function () {
    clearTimeout(typingTimeout);

    typingTimeout = setTimeout(() => {
        const search_result_element = $('.search-result-wrapper');
        const query = $(this).val();
        if (query) {
            const warehouse = getSelectedWarehouses() || null;
            $.ajax({
                type: 'GET',
                url: `/api/admin/products/search/${query}`,
                data: {
                    warehouse_id: warehouse
                },
                success: (response) => {
                    search_result_element.addClass('show');
                    search_result_element.find('.search-result-list').empty();
                    if (response.data.products.length) {
                        response.data.products.forEach((product) => {
                            const imageUrl = product.productImages[0]?.imageSrc || '/admin/images/no-image.jpg';
                            search_result_element.find('.search-result-list').append(`
                                <div class="search-result-item">
                                    <a href="/admin/products/1/${product.productId}" class="d-flex align-items-center gap-2 text-decoration-none">
                                        <div>
                                            <img src="${imageUrl}" alt="" style="width: 2.5rem" />
                                        </div>
                                        <div>
                                            <span class="text-product-name text-overflow-2" style="font-size: .8rem">${product.productName}</span>
                                        </div>
                                    </a>
                                </div>
                            `)
                        })
                    } else {
                        search_result_element.find('.search-result-list').append(`
                            <div class="w-100 d-flex align-items-center justify-content-center" style="height: 10rem">
                                <span>Không tìm thấy sản phẩm</span>
                            </div>
                        `)
                    }
                },
                error: () => {
                   
                }
            })
        }
        else {
            search_result_element.removeClass('show');
            search_result_element.find('.search-result-list').empty();
        }
    }, 500)
})
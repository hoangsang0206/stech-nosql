$('.view-specification').click(() => {
    $('.product-specs-hidden').addClass('show');
})

$('.close-product-specs').click(() => {
    $('.product-specs-hidden').removeClass('show');
})

$('.view-contents').click(function () {
    $('.product-contents-container').toggleClass('opened');
    $(this).toggleClass('opened');


    if ($('.product-contents-container').hasClass('opened')) {
        $(this).find('a').html(`
            <span>Thu gọn</span>
            <i class="fa-solid fa-chevron-up"></i>
        `)
    } else {
        $(this).find('a').html(`
            <span>Xem tất cả</span>
            <i class="fa-solid fa-chevron-down"></i>
        `)
    }
})

const activeStars = () => {
    const selected_stars = $('.rating-star-group-large .fa-star.active').toArray();
    const selected_stars_count = selected_stars.length;

    $('.rating-star-group-large .fa-star').removeClass('active');
    $('.rating-star-group-large').toArray().map((item) => {
        $(item).find('.fa-star').slice(0, selected_stars_count).addClass('active');
    })
}   

$('.rating-star-group-large .fa-star').hover(function () {
    $(this).prevAll().addBack().addClass('hover');
    $(this).nextAll().removeClass('hover');
}, function () {
    $('.rating-star-group-large .fa-star').removeClass('hover');
})

$('.rating-star-group-large .fa-star').click(function () {
    $('.rating-star-group-large .fa-star').removeClass('hover');
    $('.rating-star-group-large .fa-star').removeClass('active');
    $(this).prevAll().addBack().addClass('active');
    $(this).nextAll().removeClass('active');

    activeStars();
})

$('.reviewer-select-star .fa-star').click(() => {
    $('.post-review').addClass('show');
})


$('.click-upload-review-images').click(() => {
    $('#click-upload-images').click();
})

$('#click-upload-images').change(function () {
    const files = $(this)[0].files;

    if (files.length > 3) {
        alert('Chỉ được chọn tối đa 3 ảnh');
        $(this).val(null);
        return;
    }

    const dataTransfer = new DataTransfer();
    Array.from($('#review-images')[0].files).map((file) => {
        dataTransfer.items.add(file);
    });

    Array.from(files).map((file) => {
        dataTransfer.items.add(file);
    })

    while (dataTransfer.items.length > 3) {
        dataTransfer.items.remove(0);
    }

    $('#review-images')[0].files = dataTransfer.files;
    $('#review-images').trigger('change');
})

$('#review-images').change(function () {
    const files = $(this)[0].files;

    const preview_images = $('.post-review__images')
    preview_images.empty();

    Array.from(files).map((file) => {
        if (file.type.startsWith('image/')) {
            const reader = new FileReader();

            reader.onload = function (e) {
                preview_images.append(`
                    <div><img src="${e.target.result}" /></div>
                `)
            };

            reader.readAsDataURL(file);
        }
    })
})


$('.post-review form').submit(function (e) {
    e.preventDefault();

    const product_id = $(this).data('product');
    const rating = $(this).find('.rating-star-group-large .fa-star.active').length;
    const content = $(this).find('#review-content').val();
    const images = $(this).find('#review-images')[0].files || null;

    const reviewer_name = $(this).find('#reviewer-name').val() || null;
    const reviewer_email = $(this).find('#reviewer-email').val() || null;
    const reviewer_phone = $(this).find('#reviewer-phone').val() || null;

    const reviewerInfo = reviewer_name != null && reviewer_email != null ? {
        ReviewerName: reviewer_name,
        ReviewerEmail: reviewer_email,
        ReviewerPhone: reviewer_phone
    } : null;


    const formData = new FormData();

    formData.append('ProductId', product_id);
    formData.append('Rating', rating);
    formData.append('Content', content);

    if (reviewerInfo != null) {
        formData.append('ReviewerInfo.ReviewerName', reviewerInfo.ReviewerName);
        formData.append('ReviewerInfo.ReviewerEmail', reviewerInfo.ReviewerEmail);
        formData.append('ReviewerInfo.ReviewerPhone', reviewerInfo.ReviewerPhone);
    }


    if (images != null) {
        Array.from(images).map((file) => {
            formData.append('files', file);
        })
    }

    //for (const [key, value] of formData.entries()) {
    //    console.log(key, value);
    //}

    const submit_btn = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(submit_btn, '23px', '4px');
    $.ajax({
        url: '/api/reviews/post-review',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: (response) => {
            if (response.status) {
                hideButtonLoader(submit_btn, btn_element);
                showDialog('info', 'Gửi đánh giá thành công', 'Cảm ơn bạn đã gửi đánh giá cho sản phẩm này');
                $(this)[0].reset();
                $('.post-review__images').empty();
            } else {
                showDialog('error', 'Gửi đánh giá thất bại', response.message);
            }
        },
        error: () => {
            hideButtonLoader(submit_btn, btn_element);
        }
    })
})

const showReplyForm = (reply_button) => {
    $(reply_button).closest('.review-item').find('.review-reply-form').addClass('show');
}

const hideReplyForm = (form) => {
    $(form).removeClass('show');
}

$(document).on('click', '.reply-review:not(.not-logged-in)', function () {
    showReplyForm(this);
})

$(document).on('click', '.cancel-review-reply', function () {
    const form = $(this).closest('.review-reply-form');
    hideReplyForm(form);
    form.find('form')[0].reset();
})

$(document).on('submit', '.review-reply-form form', function (e) {
    e.preventDefault();

    const review_id = $(this).data('review');
    const product_id = $('.product-detail').data('product');
    const content = $(this).find('.review-reply-input').val();

    if (!content) {
        return;
    }
    if (content.trim().length <= 0) {
        showDialog('warning', 'Vui lòng nhập nội dung', null);
        return;
    }

    const submit_btn = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(submit_btn, '15px', '4px');

    $.ajax({
        type: 'POST',
        url: '/api/reviews/post-review-reply',
        contentType: 'application/json',
        data: JSON.stringify({
            ProductId: product_id,
            ReviewId: review_id,
            Content: content
        }),
        success: (response) => {
            hideButtonLoader(submit_btn, btn_element);
            if (response.status) {
                showDialog('info', 'Phản hồi đã được gửi', 'Cảm ơn bạn đã trả lời đánh giá này');
                $(this)[0].reset();
                hideReplyForm($(this).closest('.review-reply-form'));
            } else {
                showDialog('error', 'Gửi phản hồi thất bại', response.message);
            }
        },
        error: () => {
            hideButtonLoader(submit_btn, btn_element);
        }
    })
})



const formatDateTime = (date_str) => {
    const date = new Date(date_str);

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();

    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

const starGroupHTML = (star) => {
    let stars = '';
    for (let i = 1; i <= star; i++) {
        stars += `<i class="fa-solid fa-star"></i>`;
    }

    for (let i = 1; i <= 5 - star; i++) {
        stars += `<i class="fa-regular fa-star"></i>`;
    }

    return `
         <div class="rating-star-group">
            ${stars}
         </div>
    `;
}

const ratingSummaryItemHTML = (star, totalReviews, totalRatingStars) => {
    return `
         <div class="rating-summary-item">
            ${starGroupHTML(star)}
            <progress value="${totalReviews > 0 ? (parseFloat(totalRatingStars) / totalReviews) * 100 : 0}" max="100"></progress>
            <span>${totalRatingStars}</span>
        </div>
    `;
}

const reviewReplyHTML = (reply) => {
    const replyUser = reply.userReply;
    const replierAvatar = replyUser?.avatar ?? "/images/user-no-image.svg";
    const replierName = replyUser?.fullName ?? "Người dùng";

    const isAuthenticated = parseInt($('.product-detail').data('authenticated') || 0);

    return `
        <div class="review-reply-item">
            <div class="d-flex align-items-center gap-2">
                <div class="review-avatar lazy-loading">
                    <img lazy-src="${replierAvatar}" alt="" />
                </div>
                <strong>${replierName}</strong>
                ${replyUser?.roleId === 'admin' ?
                    `<span class="admin-badge">QTV</span>`
                    : ''
                }
            </div>
            <div class="mt-3">
                <span>${reply.content}</span>
            </div>
            <div class="d-flex align-items-center gap-4 mt-3">
                <a href="javascript:" class="d-flex align-items-center gap-1 reply-review ${!isAuthenticated ? 'not-logged-in' : ''}" data-review="${reply.id}">
                    <i class="fa-regular fa-comment-dots"></i>
                    <span>Phản hồi</span>
                </a>

                <span class="text-secondary review-time mt-1">${formatDateTime(reply.replyDate)}</span>
            </div>
        </div>
    `;
}

const reviewReplyFormHTML = (reviewId) => {
    return `
        <div class="review-reply-form mt-3">
            <form data-review="${reviewId}">
                <div>
                    <textarea style="border-radius: var(--radius); border: 1px solid var(--border-color); height: 4rem"
                        class="w-100 p-3 review-reply-input"
                        id="review-reply-@Model.ReviewId"
                        placeholder="Viết phản hồi..." required></textarea>
                </div>

                <div class="d-flex gap-2">
                    <button type="button" class="cancel-review-reply">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                    <button type="submit" class="submit-review-reply">
                        <i class="fa-regular fa-paper-plane"></i>
                    </button>
                </div>
            </form>
        </div>
    `;
}

const reviewHTML = (review) => {
    const reviewerUser = review.user;
    const reviewerAvatar = reviewerUser?.avatar ?? "/images/user-no-image.svg";
    const reviewerName = reviewerUser?.fullName ?? review.reviewerName ?? "Người dùng";

    const rating = review.rating;

    const reviewImages = review.reviewImages;
    const reviewReplies = review.reviewReplies;

    const isAuthenticated = parseInt($('.product-detail').data('authenticated') || 0);

    return `
        <div class="review-item">
            <div class="review-item-main">
                <div class="d-flex align-items-center gap-2">
                    <div class="review-avatar lazy-loading">
                        <img lazy-src="${reviewerAvatar}" alt="" />
                    </div>
                    <strong>${reviewerName}</strong>
                </div>

                <div class="mt-2 d-flex align-items-center gap-3">

                    ${starGroupHTML(rating)}

                    <div>
                        ${review.isPurchased === true ?
                            `
                                <div class="review-badge success">
                                    <i class="fa-regular fa-circle-check"></i>
                                    <span>Đã mua hàng</span>
                                </div>
                            `
                            :
                            `
                                <div class="review-badge">
                                    <span>Chưa mua hàng</span>
                                </div>
                            `
                        }
                    </div>
                </div>

                <div class="mt-3">
                    <span>${review.content}</span>
                </div>

                ${reviewImages.length ?
                    `<div class="mt-3 d-flex gap-2">
                        ${
                            reviewImages.map((image) => {
                                return `
                                    <div class="review-image lazy-loading">
                                        <img lazy-src="${image.imageUrl}" alt="" />
                                    </div>
                                `;
                            }).join('')
                        }
                    </div>`
                    : ''
                }

                <div class="d-flex align-items-center gap-4 mt-3 overflow-x-auto">
                    <a href="javascript:" class="d-flex align-items-center gap-1 ${!review.isLiked ? 'like-review' : ''} ${!isAuthenticated ? 'not-logged-in' : ''} text-decoration-none" data-review="${review.id}" data-like="${review.totalLike}">
                        ${review.isLiked ? '<i class="fa-solid fa-thumbs-up"></i>' : '<i class="fa-regular fa-thumbs-up"></i>'}
                        <span>Hữu ích (${review.totalLike})</span>
                    </a>

                    <a href="javascript:" class="d-flex align-items-center gap-1 reply-review ${!isAuthenticated ? 'not-logged-in' : ''}" data-review="${review.id}">
                        <i class="fa-regular fa-comment-dots"></i>
                        <span>Phản hồi</span>
                    </a>

                    <span class="text-secondary review-time mt-1">${formatDateTime(review.createAt)}</span>
                </div>

            </div>

            ${reviewReplies.length ?

                `<div>
                    <div class="review-replies" data-review="${review.id}">
                        ${
                            reviewReplies.map((reply) => {
                                return reviewReplyHTML(reply);
                            }).join('')
                        }
                    </div>
                    <div class="mt-2">
                        <a style="padding-left: 3rem" href="javascript:" class="text-load-more load-more-replies" data-current-page="1" data-review="${review.id}">Xem thêm phản hồi</a>
                    </div>
                </div>`

                : ''

            }

            <div class="review-reply-form-container">
                ${reviewReplyFormHTML(review.id)}
            </div>

        </div>
    `;
}

let isUpdated = false;

const updateRatingSummary = (totalReviews, overview) => {
    isUpdated = true;

    $('#avg-rating').html(`${overview.averageRating}/5`);
    $('#total-reviews').html(totalReviews > 0 ? `${totalReviews} đánh giá` : 'Chưa có đánh giá')

    if (totalReviews <= 0) {
        $('.let-review-firstly').removeClass('d-none');
    }

    $('#rating-summary').empty();
    $('#rating-summary').append(ratingSummaryItemHTML(5, totalReviews, overview.total5StarReviews));
    $('#rating-summary').append(ratingSummaryItemHTML(4, totalReviews, overview.total4StarReviews));
    $('#rating-summary').append(ratingSummaryItemHTML(3, totalReviews, overview.total3StarReviews));
    $('#rating-summary').append(ratingSummaryItemHTML(2, totalReviews, overview.total2StarReviews));
    $('#rating-summary').append(ratingSummaryItemHTML(1, totalReviews, overview.total1StarReviews));
}

const renderReviews = (data) => { 
    const currentPage = data.currentPage;
    const totalPages = data.totalPages;
    const remainingReviews = data.remainingReviews;
    const totalReviews = data.totalReviews;

    const overview = data.reviewOverview;
    const reviews = data.reviews;

    if (!isUpdated) {
        updateRatingSummary(totalReviews, overview);
    }

    reviews.map((review) => {
        $('.reviews-list').append(reviewHTML(review) + '<hr />');
    })

    if (currentPage < totalPages) {
        $('.load-more-reviews').removeClass('d-none').html(`Xem thêm đánh giá (${remainingReviews})`).data('current-page', currentPage);
    } else {
        $('.load-more-reviews').addClass('d-none').empty();
    }

    lazyLoading();
}


let first_load = true;
const loadReviews = (sort_by, filter_by, page) => { 
    if (first_load) {
        showPageContentLoader('.reviews-list', 15); //15rem
    }

    $.ajax({
        type: 'GET',
        url: '/api/reviews/get-reviews',
        data: {
            pId: $('.product-detail').data('product'),
            sort_by: sort_by,
            filter_by: filter_by,
            page: page
        },
        success: (response) => { 
            if (first_load) {
                const timeout_ms = 500; //ms
                hidePageContentLoader('.reviews-list', timeout_ms);
                const content_timeout = setTimeout(() => {
                    renderReviews(response.data);
                    clearTimeout(content_timeout);
                }, timeout_ms);
            } else {
                renderReviews(response.data);
            }
        },
        error: () => { 
            console.log("Cannot get reviews");
        }
    })

}

const renderReviewReplies = (data, reviewId) => {
    const reviewReplies = data.reviewReplies;
    const currentPage = data.currentPage;
    const totalPages = data.totalPages;
    const remainingReplies = data.remainingReplies;

    if (currentPage < totalPages) {
        $(`.load-more-replies[data-review="${reviewId}"]`).html(`Xem thêm phản hồi (${remainingReplies})`).data('current-page', currentPage);
    } else {
        $(`.load-more-replies[data-review="${reviewId}"]`).addClass('d-none').empty();
    }

    reviewReplies.map((reply) => {
        $(`.review-replies[data-review="${reply.reviewId}"]`).append(reviewReplyHTML(reply));
    })

    lazyLoading();
}

const loadReviewReplies = (review_id, page) => {
    $.ajax({
        type: 'GET',
        url: '/api/reviews/get-review-replies',
        data: {
            rId: review_id,
            page: page
        },
        success: (response) => {
            renderReviewReplies(response.data, review_id);
        },
        error: () => {
            console.log("Cannot get review replies");
        }
    })
}

$(document).ready(() => { 
    first_load = true;
    loadReviews(null, null, 1);
})

$(document).on('click', '.load-more-reviews', function () {
    const nextPage = parseInt($(this).data('current-page') || 0) + 1;
    const sort_by = $('.sort-reviews').data('selected-value');
    const filter_by = $('.filter-reviews .filter-button.active').data('filter');

    first_load = false;
    loadReviews(sort_by, filter_by, nextPage);
})

$('.filter-reviews .filter-button').not('.active').click(function () {
    $('.filter-reviews .filter-button').removeClass('active');
    $(this).addClass('active');

    const filter_by = $(this).data('filter');
    const sort_by = $('.sort-reviews').data('selected-value');
    const page = parseInt($('.load-more-reviews').data('current-page') || 0);

    first_load = true;
    $('.reviews-list').empty();
    loadReviews(sort_by, filter_by, page);
})

$(document).on('click', '.load-more-replies', function () {
    const review_id = $(this).data('review');
    const page = parseInt($(this).data('current-page') || 0) + 1;

    loadReviewReplies(review_id, page);
})



$(document).on('click', '.like-review:not(.not-logged-in)', function () {
    const review_id = $(this).data('review');
    const product_id = $('.product-detail').data('product');

    const total_like = $(this).data('like');

    $.ajax({
        type: 'POST',
        url: `/api/reviews/post-like?rId=${review_id}&pId=${product_id}`,
        success: (response) => {
            if (response.status) {
                $(this).data('like', total_like + 1);
                $(this).html(`
                    <i class="fa-solid fa-thumbs-up"></i>
                    <span>Hữu ích(${total_like + 1})</span>
                `);
                $(this).removeClass('like-review');
            }
        }
    })
})
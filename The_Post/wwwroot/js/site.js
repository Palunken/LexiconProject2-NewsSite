// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: "toast-bottom-right",    
    timeOut: 3000,
    extendedTimeOut: 1000,
   
};

// Editor's Choice slider in Jquery
$(document).ready(function () {
    $('.editors-choice-toggle').change(function () {
        var $switch = $(this);
        var articleId = $switch.data('article-id');
        var isChecked = $switch.prop('checked');

        $.ajax({
            url: '/Admin/UpdateEditorsChoice',
            type: 'POST',
            data: {
                articleId: articleId,
                isEditorsChoice: isChecked
            },
            success: function (response) {
                if (response.success) {
                    toastr.success('Editor\'s choice updated successfully');
                }
                else {
                    $switch.prop('checked', !isChecked);
                    toastr.error(response.message);
                }
            },
            error: function () {
                $switch.prop('checked', !isChecked);
                toastr.error('An error occured while updating editor\'s choice');
            }
        });
    });
});

// isArchived checkbox in JavaScript
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".is-archive-check").forEach((checkbox) => {
        checkbox.addEventListener("change", async function () {
            const articleId = parseInt(this.dataset.articleId);
            const isChecked = this.checked;

            // Get the closest list item for toggling the archived class
            const listItem = this.closest("li.list-group-item");
            if (listItem) {
                listItem.classList.toggle("archived-css", isChecked);
            }

            try {
                const response = await fetch('/Admin/ArchiveArticle', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ articleId: articleId, isArchived: isChecked })
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const result = await response.json();
                if (result.success) {
                    toastr.success("Archive status updated successfully");
                } else {
                    if (listItem) {
                        listItem.classList.toggle("archived-css", !isChecked);
                    }
                    this.checked = !isChecked;
                    toastr.error(result.message || "Failed to update archive status");
                }
            } catch (error) {
                if (listItem) {
                    listItem.classList.toggle("archived-css", !isChecked);
                }
                this.checked = !isChecked;
                toastr.error("An unexpected error occurred");
                console.error('Error:', error);
            }
        });
    });
});

 //When the document is ready, add an error event listener to all images with data-original attribute
 //This event listener will be triggered when an image fails to load
 //If the image fails to load, it will use the original image source instead
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("img[data-original]").forEach(img => {
        img.onerror = function () {
            console.log("Image not found, using original image instead.");
            let originalSrc = img.getAttribute("data-original");
            if (originalSrc) {
                img.src = originalSrc;
            }
        };
    });
});


/*----- Newsletter subscription form --------*/
document.addEventListener("DOMContentLoaded", function () {
    const subscribeCheckbox = document.getElementById("subscribeCheckbox");

    // If the subscribe checkbox is not found, this script will not run
    // This is to prevent errors on pages where the checkbox is not present
    if (!subscribeCheckbox) {
        return; 
    }

    const categoryCheckboxes = document.querySelectorAll(".categoryCheckbox"); // Includes the editors choice checkbox
    const saveButton = document.querySelector('input[type="submit"]');
    const categoriesLabel = document.querySelector(".newsletterSelections > label.fs-5.fw-bold");

    // Function to enable/disable category checkboxes based on subscription
    function toggleNewsletterSelections() {
        const isSubscribed = subscribeCheckbox.checked;

        categoryCheckboxes.forEach((checkbox) => {
            checkbox.disabled = !isSubscribed;
            if (!isSubscribed) {
                checkbox.checked = false; // Uncheck all boxes if not subscribed
            }
            const label = checkbox.closest(".d-flex")?.querySelector("label"); // Find label for each checkbox
            if (label) {
                label.classList.toggle("greyed-out", checkbox.disabled); // Add "greyed-out" css class if checkbox is disabled
            }
        });

        categoriesLabel.classList.toggle("greyed-out", !isSubscribed); // Greys out the label for categories if not subscribed

        updateSaveButtonState(); // Re-check button state when toggling subscription
    }

    // Function to update the Save button's disabled state
    function updateSaveButtonState() {
        const isSubscribed = subscribeCheckbox.checked;
        const anyChecked = Array.from(categoryCheckboxes).some(checkbox => checkbox.checked);
        if (!isSubscribed || anyChecked) {
            saveButton.disabled = false; // Enable save button if subscribed or any checkbox is checked
        } else {
            saveButton.disabled = true; // Disable save button if not subscribed and no category is checked
        }
    }

    // Initialize the form on load
    toggleNewsletterSelections(); // Call the function on load to set the initial state

    // Event listener to toggle the newsletter selections when the subscribe checkbox changes
    subscribeCheckbox.addEventListener("change", toggleNewsletterSelections);

    // Event listener to handle any checkbox change to update the save button state
    categoryCheckboxes.forEach((checkbox) => {
        checkbox.addEventListener("change", updateSaveButtonState);
    });
});

// Cookie notice
document.addEventListener("DOMContentLoaded", function () {

    if (document.cookie.includes("cookiesConsent=true")) {
        document.getElementById('cookie-notice').style.display = 'none';
    }

    const acceptCookiesBtn = document.getElementById('accept-cookies-btn');

    // If the button is rendered, i. e. if cookies haven't been accepted yet
    if (acceptCookiesBtn) {
        // Handle Accept
        acceptCookiesBtn.onclick = async function () {
            try {
                let response = await fetch('/BaseCookies/AcceptCookies', { method: 'POST' });
                if (response.ok) {
                    document.getElementById('cookie-notice').style.display = 'none';
                } else {
                    console.error("Failed to accept cookies");
                }
            } catch (error) {
                console.error("Network error:", error);
            }
        };
    }

    // Handle both Ignore buttons
    const ignoreButtons = ['ignore-cookies-btn', 'ignore-cookies-btn-header'];
    ignoreButtons.forEach(id => {
        const button = document.getElementById(id);
        if (button) {
            button.onclick = function () {
                document.getElementById('cookie-notice').style.display = 'none';
            };
        }
    });
});



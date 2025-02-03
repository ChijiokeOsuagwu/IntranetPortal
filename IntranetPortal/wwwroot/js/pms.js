
//===== Function to delete a submission message ========//
function deleteSubmission(submission_id) {
    $.ajax({
        type: 'POST',
        url: '/PMS/Process/DeleteSubmission',
        dataType: "text",
        data: { sd: submission_id },
        success: function (result) {
            if (result == "deleted") {
                location.reload();
            }
            else {
                alert('Deleting record failed!');
                console.log(result);
            }
        },
        error: function () {
            alert('Sorry deleting operation could not be completed.');
            console.log('Failed ');
        }
    })
}

//===== Function to mark a submission message as 'Actioned' ========//
function markDone(submission_id) {
    //const btnRevoke = document.getElementById("btn_revoke_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/PMS/Process/MarkSubmissionAsDone',
        dataType: "text",
        data: { sd: submission_id },
        success: function (result) {
            if (result == "marked") {
                location.reload();
            }
            else {
                alert('Updating action status failed!');
            }
        },
        error: function () {
            alert('Sorry action status was not updated.');
            console.log('Failed ');
        }
    })
}

//===== Function to save Appraiser KPA Score to the database ========//
function saveKpaScore(review_result_id, review_header_id, review_metric_id, appraiser_id, primary_appraiser_id, submission_id) {
    const spanSaved = document.getElementById("span_" + review_metric_id);
    const actualAchievementTextArea = document.getElementById("txt_actual_achievement_" + review_metric_id);
    const appraiserCommentTextArea = document.getElementById("txt_appraiser_comment_" + review_metric_id);
    const appraiserScoreTextBox = document.getElementById("txt_appraiser_score_" + review_metric_id);

    //== validation labels==//
    const actualAchievementValidationLabel = document.getElementById("validation_lbl_actual_achievement_" + review_metric_id);
    const scoreValidationLabel = document.getElementById("validation_lbl_appraiser_score_" + review_metric_id);

    let actual_achievement = actualAchievementTextArea.value;
    let appraiser_comment = appraiserCommentTextArea.value;
    let appraiser_score = appraiserScoreTextBox.value;

    if (actual_achievement === null || actual_achievement.trim().length === 0) {
        actualAchievementValidationLabel.innerHTML = "<span style='color:#FF0000; background-color:#cfd8dc'>Please enter Actual Achievement!</span>";
        actualAchievementTextArea.focus();
        return;
    }
    actualAchievementValidationLabel.innerHTML = "";

    if (isNaN(appraiser_score) || appraiser_score === undefined || appraiser_score < 1 || appraiser_score > 100) {
        scoreValidationLabel.innerHTML = "<span style='color:#FF0000; background-color:#cfd8dc'>Please enter a valid score!</span>";
        appraiserScoreTextBox.focus();
        return;
    }
    scoreValidationLabel.innerHTML = "";

    $.ajax({
        type: 'POST',
        url: '/PMS/Process/EvaluateKpa',
        dataType: "text",
        data: { rh: review_header_id, rm: review_metric_id, ap: appraiser_id, aa: actual_achievement, ac: appraiser_comment, sc: appraiser_score, pa: primary_appraiser_id, id: review_result_id, sd: submission_id },
        success: function (result) {
            if (result == "saved") {
                spanSaved.innerHTML = "<h5 style='color:#66FF00'>Saved successfully <i class='bi bi-check-lg'></i></h5>";
                //location.reload();
            }
            else if (result == "failed") {
                spanSaved.innerHTML = "<h5 style='color:#FF0000'>Not Saved. Please try again. <i class='bi bi-x-lg'></i></h5>";
            }
            else {
                spanSaved.innerHTML = "<h5 style='color:#66FF00'>An error encountered. Please try again. <i class=bi bi-exclamation-lg></i></h5>";
                alert(result);
            }
        },
        error: function (err) {
            spanSaved.innerHTML = '<h5 class="text-warning">Error encountered<i class="bi bi-exclamation-lg"></i></h5>';
            console.log(err);
        }
    })
}

//===== Function to save Appraiser Competency Score to the database ========//
function saveCmpScore(review_result_id, review_header_id, review_metric_id, appraiser_id, primary_appraiser_id, submission_id) {
    const spanSaved = document.getElementById("span_" + review_metric_id);
    const appraiserCommentTextArea = document.getElementById("txt_appraiser_comment_" + review_metric_id);
    const appraiserScoreDropdown = document.getElementById("dpd_appraiser_score_" + review_metric_id);

    let appraiser_comment = appraiserCommentTextArea.value;
    let appraiser_score = parseFloat(appraiserScoreDropdown.value).toFixed(2);

    $.ajax({
        type: 'POST',
        url: '/PMS/Process/EvaluateCmp',
        dataType: "text",
        data: { rh: review_header_id, rm: review_metric_id, ap: appraiser_id, sc: appraiser_score, ac: appraiser_comment, pa: primary_appraiser_id, id: review_result_id, sd: submission_id },
        success: function (result) {
            if (result == "saved") {
                spanSaved.innerHTML = "<h5 style='color:#66FF00'>Saved successfully <i class='bi bi-check-lg'></i></h5>";
            }
            else if (result == "failed") {
                spanSaved.innerHTML = "<h5 style='color:#FF0000'>Not Saved. Please try again. <i class='bi bi-x-lg'></i></h5>";
            }
            else {
                spanSaved.innerHTML = "<h5 style='color:#FF0000'>Error encountered <i class='bi bi-exclamation-lg'></i></h5>";
                console.log(result);
            }
        },
        error: function (err) {
            spanSaved.innerHTML = '<h5 style="color:#FF0000">Error encountered<i class="bi bi-exclamation-lg"></i></h5>';
            console.log(err);
        }
    })
}

$("#nm").autocomplete(
    {
        minLength: 3,
        source: function (request, response) {
            var text = $("#nm").val();
            $.ajax({
                type: "GET",
                url: "/ERM/Home/GetEmployeeNames?name=" + text,
                data: { text: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item, value: item }
                    }))
                }
            })
        }
    })
using UniSphere.Api.Controllers;

namespace UniSphere.Api.Helpers;

public static class BilingualErrorMessages
{
    // Authentication & Authorization
    public static string GetUnauthorizedMessage(Languages lang) => lang switch
    {
        Languages.En => "You are not authorized to access this resource.",
        Languages.Ar => "غير مصرح لك بالوصول إلى هذا المورد.",
        _ => "You are not authorized to access this resource."
    };

    public static string GetForbiddenMessage(Languages lang) => lang switch
    {
        Languages.En => "Access to this resource is forbidden.",
        Languages.Ar => "الوصول إلى هذا المورد محظور.",
        _ => "Access to this resource is forbidden."
    };

    public static string GetTokenExpiredMessage(Languages lang) => lang switch
    {
        Languages.En => "Token has expired.",
        Languages.Ar => "انتهت صلاحية الرمز المميز.",
        _ => "Token has expired."
    };

    public static string GetInvalidCodeMessage(Languages lang) => lang switch
    {
        Languages.En => "Code is not valid.",
        Languages.Ar => "الرمز غير صالح.",
        _ => "Code is not valid."
    };

    public static string GetPasswordMismatchMessage(Languages lang) => lang switch
    {
        Languages.En => "Password and ConfirmPassword must be the same.",
        Languages.Ar => "يجب أن تكون كلمة المرور وتأكيد كلمة المرور متطابقتين.",
        _ => "Password and ConfirmPassword must be the same."
    };

    public static string GetStudentNotRegisteredMessage(Languages lang) => lang switch
    {
        Languages.En => "Student not registered.",
        Languages.Ar => "الطالب غير مسجل.",
        _ => "Student not registered."
    };

    public static string GetWrongPasswordMessage(Languages lang) => lang switch
    {
        Languages.En => "Wrong password.",
        Languages.Ar => "كلمة المرور غير صحيحة.",
        _ => "Wrong password."
    };

    public static string GetErrorCreatingStudentMessage(Languages lang) => lang switch
    {
        Languages.En => "Error creating student.",
        Languages.Ar => "حدث خطأ أثناء إنشاء الطالب.",
        _ => "Error creating student."
    };

    public static string GetErrorCreatingSuperAdminMessage(Languages lang) => lang switch
    {
        Languages.En => "Error creating super admin.",
        Languages.Ar => "حدث خطأ أثناء إنشاء المسؤول الأعلى.",
        _ => "Error creating super admin."
    };

    public static string GetSuperAdminNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Super admin not found.",
        Languages.Ar => "لم يتم العثور على المسؤول الأعلى.",
        _ => "Super admin not found."
    };

    public static string GetUserNotSuperAdminMessage(Languages lang) => lang switch
    {
        Languages.En => "User is not a super admin.",
        Languages.Ar => "المستخدم ليس مسؤولاً أعلى.",
        _ => "User is not a super admin."
    };

    public static string GetNoImageFileMessage(Languages lang) => lang switch
    {
        Languages.En => "No image file provided.",
        Languages.Ar => "لم يتم توفير ملف صورة.",
        _ => "No image file provided."
    };

    public static string GetInvalidImageFormatMessage(Languages lang) => lang switch
    {
        Languages.En => "Invalid image format. Allowed formats: jpg, jpeg, png, gif, bmp, webp.",
        Languages.Ar => "تنسيق الصورة غير صالح. التنسيقات المسموح بها: jpg, jpeg, png, gif, bmp, webp.",
        _ => "Invalid image format. Allowed formats: jpg, jpeg, png, gif, bmp, webp."
    };

    public static string GetImageFileSizeTooLargeMessage(Languages lang) => lang switch
    {
        Languages.En => "Image file size must be less than 5MB.",
        Languages.Ar => "يجب أن يكون حجم ملف الصورة أقل من 5 ميجابايت.",
        _ => "Image file size must be less than 5MB."
    };

    public static string GetProfileImageUploadedMessage(Languages lang) => lang switch
    {
        Languages.En => "Profile image uploaded successfully.",
        Languages.Ar => "تم رفع صورة الملف الشخصي بنجاح.",
        _ => "Profile image uploaded successfully."
    };

    // Not Found Messages
    public static string GetNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "The requested resource was not found.",
        Languages.Ar => "لم يتم العثور على المورد المطلوب.",
        _ => "The requested resource was not found."
    };

    public static string GetSubjectNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Subject not found.",
        Languages.Ar => "لم يتم العثور على المادة.",
        _ => "Subject not found."
    };

    public static string GetStudentNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Student not found.",
        Languages.Ar => "لم يتم العثور على الطالب.",
        _ => "Student not found."
    };

    public static string GetScheduleNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Schedule not found.",
        Languages.Ar => "لم يتم العثور على الجدول الدراسي.",
        _ => "Schedule not found."
    };

    public static string GetLectureNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Lecture not found.",
        Languages.Ar => "لم يتم العثور على المحاضرة.",
        _ => "Lecture not found."
    };

    public static string GetMajorNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Major not found.",
        Languages.Ar => "لم يتم العثور على التخصص.",
        _ => "Major not found."
    };

    public static string GetFacultyNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Faculty not found.",
        Languages.Ar => "لم يتم العثور على الكلية.",
        _ => "Faculty not found."
    };

    // Validation Messages
    public static string GetStudentGradesEmptyMessage(Languages lang) => lang switch
    {
        Languages.En => "StudentGrades list cannot be empty.",
        Languages.Ar => "قائمة درجات الطلاب لا يمكن أن تكون فارغة.",
        _ => "StudentGrades list cannot be empty."
    };

    public static string GetSubjectIdRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "SubjectId is required.",
        Languages.Ar => "معرف المادة مطلوب.",
        _ => "SubjectId is required."
    };

    public static string GetPassGradeRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "PassGrade is required.",
        Languages.Ar => "درجة النجاح مطلوبة.",
        _ => "PassGrade is required."
    };

    public static string GetMajorIdRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "MajorId is required.",
        Languages.Ar => "معرف التخصص مطلوب.",
        _ => "MajorId is required."
    };

    public static string GetYearRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "Year is required.",
        Languages.Ar => "السنة مطلوبة.",
        _ => "Year is required."
    };

    public static string GetScheduleDateRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "Schedule date is required.",
        Languages.Ar => "تاريخ الجدول الدراسي مطلوب.",
        _ => "Schedule date is required."
    };

    public static string GetLecturesRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "Lectures list is required.",
        Languages.Ar => "قائمة المحاضرات مطلوبة.",
        _ => "Lectures list is required."
    };

    public static string GetFileRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "File is required.",
        Languages.Ar => "الملف مطلوب.",
        _ => "File is required."
    };

    public static string GetFileUploadErrorMessage(Languages lang) => lang switch
    {
        Languages.En => "Error uploading file.",
        Languages.Ar => "خطأ في رفع الملف.",
        _ => "Error uploading file."
    };

    // Business Logic Messages
    public static string GetStudentsNotEnrolledMessage(Languages lang, string studentIds) => lang switch
    {
        Languages.En => $"Some students are not enrolled in the subject: {studentIds}",
        Languages.Ar => $"بعض الطلاب غير مسجلين في المادة: {studentIds}",
        _ => $"Some students are not enrolled in the subject: {studentIds}"
    };

    public static string GetScheduleAlreadyExistsMessage(Languages lang) => lang switch
    {
        Languages.En => "A schedule already exists for this date, major, and year.",
        Languages.Ar => "يوجد جدول دراسي بالفعل لهذا التاريخ والتخصص والسنة.",
        _ => "A schedule already exists for this date, major, and year."
    };

    public static string GetNoGradesFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "No grades found for this student.",
        Languages.Ar => "لم يتم العثور على درجات لهذا الطالب.",
        _ => "No grades found for this student."
    };

    public static string GetNoAccessToSubjectMessage(Languages lang) => lang switch
    {
        Languages.En => "You do not have access to this subject.",
        Languages.Ar => "ليس لديك صلاحية الوصول إلى هذه المادة.",
        _ => "You do not have access to this subject."
    };

    // Success Messages
    public static string GetSuccessMessage(Languages lang) => lang switch
    {
        Languages.En => "Operation completed successfully.",
        Languages.Ar => "تم إكمال العملية بنجاح.",
        _ => "Operation completed successfully."
    };

    public static string GetCreatedMessage(Languages lang) => lang switch
    {
        Languages.En => "Resource created successfully.",
        Languages.Ar => "تم إنشاء المورد بنجاح.",
        _ => "Resource created successfully."
    };

    public static string GetUpdatedMessage(Languages lang) => lang switch
    {
        Languages.En => "Resource updated successfully.",
        Languages.Ar => "تم تحديث المورد بنجاح.",
        _ => "Resource updated successfully."
    };

    public static string GetDeletedMessage(Languages lang) => lang switch
    {
        Languages.En => "Resource deleted successfully.",
        Languages.Ar => "تم حذف المورد بنجاح.",
        _ => "Resource deleted successfully."
    };

    // Generic Error Messages
    public static string GetInternalServerErrorMessage(Languages lang) => lang switch
    {
        Languages.En => "An internal server error occurred. Please try again later.",
        Languages.Ar => "حدث خطأ داخلي في الخادم. يرجى المحاولة مرة أخرى لاحقاً.",
        _ => "An internal server error occurred. Please try again later."
    };

    public static string GetBadRequestMessage(Languages lang) => lang switch
    {
        Languages.En => "The request is invalid.",
        Languages.Ar => "الطلب غير صحيح.",
        _ => "The request is invalid."
    };

    public static string GetSuperAdminGmailExistsMessage(Languages lang) => lang switch
    {
        Languages.En => "A SuperAdmin with this Gmail already exists.",
        Languages.Ar => "يوجد مسؤول أعلى بهذا البريد الإلكتروني بالفعل.",
        _ => "A SuperAdmin with this Gmail already exists."
    };

    public static string GetCannotRemoveSuperAdminWithAccountMessage(Languages lang) => lang switch
    {
        Languages.En => "Cannot remove SuperAdmin with registered account. Delete the account first.",
        Languages.Ar => "لا يمكن إزالة المسؤول الأعلى بحساب مسجل. احذف الحساب أولاً.",
        _ => "Cannot remove SuperAdmin with registered account. Delete the account first."
    };

    public static string GetSuperAdminRemovedMessage(Languages lang) => lang switch
    {
        Languages.En => "SuperAdmin removed successfully.",
        Languages.Ar => "تمت إزالة المسؤول الأعلى بنجاح.",
        _ => "SuperAdmin removed successfully."
    };

    public static string GetOneTimeCodeAssignedMessage(Languages lang) => lang switch
    {
        Languages.En => "One-time code assigned successfully to super admin.",
        Languages.Ar => "تم تعيين رمز لمرة واحدة للمسؤول الأعلى بنجاح.",
        _ => "One-time code assigned successfully to super admin."
    };

    public static string GetCannotDeleteEnrollmentStatusMessage(Languages lang) => lang switch
    {
        Languages.En => "Cannot delete enrollment status that is being used by students.",
        Languages.Ar => "لا يمكن حذف حالة التسجيل المستخدمة من قبل الطلاب.",
        _ => "Cannot delete enrollment status that is being used by students."
    };

    public static string GetEnrollmentStatusRemovedMessage(Languages lang) => lang switch
    {
        Languages.En => "Enrollment status removed successfully.",
        Languages.Ar => "تمت إزالة حالة التسجيل بنجاح.",
        _ => "Enrollment status removed successfully."
    };

    public static string GetLabNotFoundMessage(Languages lang) => lang switch
    {
        Languages.En => "Lab not found.",
        Languages.Ar => "لم يتم العثور على المختبر.",
        _ => "Lab not found."
    };

    // Material Upload Messages
    public static string GetMaterialUploadRequiredMessage(Languages lang) => lang switch
    {
        Languages.En => "Either a file or a link must be provided.",
        Languages.Ar => "يجب توفير ملف أو رابط.",
        _ => "Either a file or a link must be provided."
    };

    public static string GetMaterialUploadErrorMessage(Languages lang) => lang switch
    {
        Languages.En => "Error uploading material.",
        Languages.Ar => "حدث خطأ أثناء رفع المادة.",
        _ => "Error uploading material."
    };

    public static string GetMaterialUploadSuccessMessage(Languages lang) => lang switch
    {
        Languages.En => "Material uploaded successfully.",
        Languages.Ar => "تم رفع المادة بنجاح.",
        _ => "Material uploaded successfully."
    };

    public static string GetInvalidLinkFormatMessage(Languages lang) => lang switch
    {
        Languages.En => "Invalid link format.",
        Languages.Ar => "تنسيق الرابط غير صالح.",
        _ => "Invalid link format."
    };

    // Faculty Announcement Image Messages
    public static string GetFacultyAnnouncementImageUploadErrorMessage(Languages lang) => lang switch
    {
        Languages.En => "Error uploading faculty announcement image.",
        Languages.Ar => "حدث خطأ أثناء رفع صورة إعلان الكلية.",
        _ => "Error uploading faculty announcement image."
    };

    public static string GetFacultyAnnouncementImageUploadSuccessMessage(Languages lang) => lang switch
    {
        Languages.En => "Faculty announcement image uploaded successfully.",
        Languages.Ar => "تم رفع صورة إعلان الكلية بنجاح.",
        _ => "Faculty announcement image uploaded successfully."
    };

    public static string GetFacultyAnnouncementCreatedWithImagesMessage(Languages lang) => lang switch
    {
        Languages.En => "Faculty announcement created successfully with images.",
        Languages.Ar => "تم إنشاء إعلان الكلية بنجاح مع الصور.",
        _ => "Faculty announcement created successfully with images."
    };
} 
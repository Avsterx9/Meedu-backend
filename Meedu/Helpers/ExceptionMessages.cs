namespace Meedu.Helpers;

public static class ExceptionMessages
{
    // USERS
    public static readonly string UserNotFound = "User not found";
    public static readonly string InvalidUsernameOrPassword = "Invalid username or password";

    // FILES
    public static readonly string FileIsEmpty = "File is empty";

    // LESSON OFFERS
    public static readonly string LessonOfferNotFound = "Lesson offer not found";

    // SCHEDULES
    public static readonly string ScheduleAlreadyExists = "Schedule already exists";
    public static readonly string ScheduleNotFound = "Schedule not found";

    // TIMESTAMPS
    public static readonly string TimestampNotAvailable = "Timestamp not available";
    public static readonly string TimestampNotFound = "Timestamp not found";
    public static readonly string InvalidTimestamp = "Invalid timestamp";

    // RESERVATIONS
    public static readonly string YouHaveLessonReservedAtThisTime = "You have lesson reserved at this time";
    public static readonly string DateIsAlreadyReserved = "Date is already reserved";
    public static readonly string ReservationDayIsIncorrect = "Reservation day is incorrect";
    public static readonly string ReservationNotFound = "Reservation npt found";

    // SUBJECTS
    public static readonly string SubjectNotFound = "Subject offer not found";
    public static readonly string SubjectAlreadyExists = "Subject already Exists";
}

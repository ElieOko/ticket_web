export class DateFormatter {

  static shortFormat(date: Date): string{
    var year = `${date.getFullYear()}`;
    var month = `${date.getMonth() + 1}`;
    var day = `${date.getDate() < 10 ? '0' + date.getDate() : date.getDate()}`;
    return `${year}-${month}-${day}`;
  }

}

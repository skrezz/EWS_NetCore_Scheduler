declare global {
  interface Date {
      toISODate(): string;
  }
}

Date.prototype.toISODate = function () : string {
  return (
    this.getFullYear() +
    "-" +
    ("0" + (this.getMonth() + 1)).slice(-2) +
    "-" +
    ("0" + this.getDate()).slice(-2)
  );
};

export {}

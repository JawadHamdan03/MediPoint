using MediPoint.Application.Common.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediPoint.Infrastructure.Common.Services;

public class QuestPdfPrescriptionGenerator : IPrescriptionPdfGenerator
{
    public byte[] Generate(PrescriptionPdfModel model)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                page.Header().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("MediPoint").FontSize(20).Bold().FontColor(Colors.Purple.Darken2);
                            c.Item().Text("Prescription").FontSize(12).FontColor(Colors.Grey.Darken1);
                        });
                        row.ConstantItem(160).AlignRight().Text(t =>
                        {
                            t.Span("Issued: ").SemiBold();
                            t.Span(model.IssuedAt.ToString("MMM d, yyyy h:mm tt"));
                        });
                    });
                    column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingVertical(16).Column(column =>
                {
                    column.Spacing(16);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Prescribing Doctor").FontSize(9).FontColor(Colors.Grey.Darken1).Bold();
                            c.Item().Text($"Dr. {model.DoctorName}").FontSize(12).SemiBold();
                            c.Item().Text(model.DoctorSpecialty);
                            c.Item().Text($"License No: {model.DoctorLicenseNumber}");
                        });
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Patient").FontSize(9).FontColor(Colors.Grey.Darken1).Bold();
                            c.Item().Text(model.PatientName).FontSize(12).SemiBold();
                            c.Item().Text($"Date of birth: {model.PatientDateOfBirth:MMM d, yyyy}");
                            c.Item().Text($"Visit date: {model.AppointmentDate:MMM d, yyyy}");
                        });
                    });

                    column.Item().Column(c =>
                    {
                        c.Item().Text("Diagnosis").FontSize(9).FontColor(Colors.Grey.Darken1).Bold();
                        c.Item().Text(model.Diagnosis);
                    });

                    if (model.Medicines.Count > 0)
                    {
                        column.Item().Column(c =>
                        {
                            c.Item().Text("Medicines").FontSize(11).Bold();
                            c.Item().PaddingTop(4).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderCell).Text("Name");
                                    header.Cell().Element(HeaderCell).Text("Dosage");
                                    header.Cell().Element(HeaderCell).Text("Frequency");
                                    header.Cell().Element(HeaderCell).Text("Days");
                                    header.Cell().Element(HeaderCell).Text("Instructions");
                                });

                                foreach (var med in model.Medicines)
                                {
                                    table.Cell().Element(BodyCell).Text(med.Name);
                                    table.Cell().Element(BodyCell).Text(med.Dosage);
                                    table.Cell().Element(BodyCell).Text(med.Frequency);
                                    table.Cell().Element(BodyCell).Text(med.DurationDays.ToString());
                                    table.Cell().Element(BodyCell).Text(med.Instructions);
                                }
                            });
                        });
                    }

                    if (model.LabResults.Count > 0)
                    {
                        column.Item().Column(c =>
                        {
                            c.Item().Text("Lab Results").FontSize(11).Bold();
                            c.Item().PaddingTop(4).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderCell).Text("Test");
                                    header.Cell().Element(HeaderCell).Text("Result");
                                    header.Cell().Element(HeaderCell).Text("Unit");
                                    header.Cell().Element(HeaderCell).Text("Reference Range");
                                });

                                foreach (var lab in model.LabResults)
                                {
                                    table.Cell().Element(BodyCell).Text(lab.TestName);
                                    table.Cell().Element(BodyCell).Text(lab.Result);
                                    table.Cell().Element(BodyCell).Text(lab.Unit);
                                    table.Cell().Element(BodyCell).Text(lab.ReferenceRange);
                                }
                            });
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(model.Notes))
                    {
                        column.Item().Column(c =>
                        {
                            c.Item().Text("Notes").FontSize(9).FontColor(Colors.Grey.Darken1).Bold();
                            c.Item().Text(model.Notes);
                        });
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Generated by MediPoint · ").FontSize(8).FontColor(Colors.Grey.Medium);
                    t.Span("For informational purposes only; consult your pharmacist before use.")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .DefaultTextStyle(x => x.SemiBold().FontSize(9).FontColor(Colors.White))
            .Background(Colors.Purple.Darken1)
            .PaddingVertical(5)
            .PaddingHorizontal(4);
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .PaddingHorizontal(4);
    }
}

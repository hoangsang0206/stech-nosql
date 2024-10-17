using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Layout.Borders;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Kernel.Colors;
using STech.Data.MongoViewModels;

namespace STech.Utils
{
    public class PrintInvoiceUtils
    {
        public static byte[] CreatePdf(IWebHostEnvironment env, InvoiceMVM invoice)
        {
            string imgPath = Path.Combine(env.WebRootPath, "images/logo/stech-logo-text-color.png");
            byte[] file;

            using (MemoryStream memoryStream = new MemoryStream())
            using (PdfWriter writer = new PdfWriter(memoryStream))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf))
            {
                PdfFont font = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\times.ttf", PdfEncodings.IDENTITY_H);

                Image image = new Image(ImageDataFactory.Create(imgPath));
                float[] colwidth = { 200f, 400f };

                Table table = new Table(colwidth);

                Cell cell11 = new Cell(1, 1)
                    .Add(image.SetWidth(150))
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                Cell cell12 = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .Add(new Paragraph("STech - High End PC & Gaming Gear").SetBold().SetFontColor(ColorConstants.RED))
                    .Add(new Paragraph("Địa chỉ: 140 Lê Trọng Tấn, P.Tây Thạnh, Q.Tân Phú, Tp.HCM"))
                    .Add(new Paragraph("Website:    "))
                    .Add(new Paragraph("Hotline:     0123456789"));

                Cell cell2 = new Cell(1, 2)
                    .SetFontSize(14)
                    .SetBold()
                    .Add(new Paragraph("HÓA ĐƠN BÁN HÀNG"))
                    .Add(new Paragraph("(Kiêm phiếu bảo hành)").SetFontSize(10).SetItalic())
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddings(15, 0, 15, 0);

                Table customerTable = new Table(new float[] { 100f, 340f, 160f });
                Cell cusCell1 = new Cell(1, 1)
                    .Add(new Paragraph("Khách hàng:"));
                Cell cusCell11 = new Cell(1, 1)
                    .Add(new Paragraph(invoice.RecipientName));
                Cell cusCell111 = new Cell(1, 1)
                   .Add(new Paragraph("Ngày: " + invoice.OrderDate?.ToString("dd/MM/yyyy")));

                Cell cusCell2 = new Cell(1, 1)
                   .Add(new Paragraph("Số ĐT:"));
                Cell cusCell21 = new Cell(1, 1)
                    .Add(new Paragraph(invoice.RecipientPhone));
                Cell cusCell211 = new Cell(1, 1)
                    .Add(new Paragraph("Số: " + invoice.InvoiceId));

                Cell cusCell3 = new Cell(1, 1)
                  .Add(new Paragraph("Đ/c nhận hàng:"));
                Cell cusCell31 = new Cell(1, 2)
                .Add(new Paragraph(invoice.DeliveryAddress));

                Cell cusCell4 = new Cell(1, 1)
                  .Add(new Paragraph("HT thanh toán:"));
                Cell cusCell41 = new Cell(1, 2)
                .Add(new Paragraph(invoice.PaymentMethod?.PaymentName));
                Cell cusCell5 = new Cell(1, 1)
                 .Add(new Paragraph("Ghi chú:"));
                Cell cusCell51 = new Cell(1, 2)
                .Add(new Paragraph(invoice.Note ?? ""));

                customerTable.AddCell(cusCell1.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell11.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell111.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell2.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell21.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell211.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell3.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell31.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell4.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell41.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell5.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(cusCell51.SetBorder(Border.NO_BORDER));
                customerTable.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));

                Table productTable = new Table(new float[] { 30f, 300f, 30f, 30f, 100f, 110f });
                Cell pCell1 = new Cell(1, 1).Add(new Paragraph("STT"));
                Cell pCell11 = new Cell(1, 1).Add(new Paragraph("Sản phẩm"));
                Cell pCell111 = new Cell(1, 1).Add(new Paragraph("BH"));
                Cell pCell1111 = new Cell(1, 1).Add(new Paragraph("SL"));
                Cell pCell11111 = new Cell(1, 1).Add(new Paragraph("Đơn giá"));
                Cell pCell111111 = new Cell(1, 1).Add(new Paragraph("Thành tiền"));

                productTable.AddCell(pCell1.SetBold().SetPadding(4).SetTextAlignment(TextAlignment.CENTER));
                productTable.AddCell(pCell11.SetBold().SetPadding(4).SetTextAlignment(TextAlignment.CENTER));
                productTable.AddCell(pCell111.SetBold().SetPadding(4).SetTextAlignment(TextAlignment.CENTER));
                productTable.AddCell(pCell1111.SetBold().SetPadding(4).SetTextAlignment(TextAlignment.CENTER));
                productTable.AddCell(pCell11111.SetBold().SetPadding(4).SetTextAlignment(TextAlignment.CENTER));
                productTable.AddCell(pCell111111.SetBold().SetPadding(4).SetTextAlignment(TextAlignment.CENTER));

                List<InvoiceDetailMVM> orderDetails = invoice.InvoiceDetails.ToList();

                for (int i = 0; i < orderDetails.Count(); i++)
                {
                    Cell pCell2 = new Cell(1, 1).Add(new Paragraph((i + 1).ToString()));
                    Cell pCell21 = new Cell(1, 1).Add(new Paragraph(orderDetails[i].Product.ProductName));
                    Cell pCell211 = new Cell(1, 1).Add(new Paragraph((orderDetails[i].Product.Warranty ?? 0).ToString()));
                    Cell pCell2111 = new Cell(1, 1).Add(new Paragraph(orderDetails[i].Quantity.ToString()));
                    Cell pCell21111 = new Cell(1, 1)
                        .Add(new Paragraph(CurrencyFormatter.Format(orderDetails[i].Cost)));
                    Cell pCell211111 = new Cell(1, 1)
                        .Add(new Paragraph(CurrencyFormatter.Format(orderDetails[i].Cost * orderDetails[i].Quantity)));

                    productTable.AddCell(pCell2
                        .SetPadding(4)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE));
                    productTable.AddCell(pCell21.SetPadding(4));
                    productTable.AddCell(pCell211
                        .SetPadding(4)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE));
                    productTable.AddCell(pCell2111
                        .SetPadding(4)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE));
                    productTable.AddCell(pCell21111
                        .SetPadding(4)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE));
                    productTable.AddCell(pCell211111
                        .SetPadding(4)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE));
                }

                Table totalTable = new Table(new float[] { 200f, 400f });
                Cell ttCel1 = new Cell(1, 1)
                    .Add(new Paragraph("Tổng tiền: ").SetTextAlignment(TextAlignment.LEFT))
                    .SetBorderRight(Border.NO_BORDER);
                Cell ttCel11 = new Cell(1, 1)
                .Add(new Paragraph(CurrencyFormatter.Format(invoice.SubTotal)).SetTextAlignment(TextAlignment.RIGHT))
                .SetBorderLeft(Border.NO_BORDER);
                Cell ttCel2 = new Cell(1, 1)
                    .Add(new Paragraph("Phí vận chuyển: ").SetTextAlignment(TextAlignment.LEFT))
                    .SetBorderRight(Border.NO_BORDER);
                Cell ttCel21 = new Cell(1, 1)
                    .Add(new Paragraph(CurrencyFormatter.Format(invoice.PackingSlip?.DeliveryFee)).SetTextAlignment(TextAlignment.RIGHT))
                    .SetBorderLeft(Border.NO_BORDER);
                Cell ttCel3 = new Cell(1, 1)
                    .Add(new Paragraph("Tổng thanh toán: ").SetTextAlignment(TextAlignment.LEFT))
                    .SetBorderRight(Border.NO_BORDER);
                Cell ttCel31 = new Cell(1, 1)
                .Add(new Paragraph(CurrencyFormatter.Format(invoice.Total)).SetTextAlignment(TextAlignment.RIGHT))
                .SetBorderLeft(Border.NO_BORDER);

                totalTable.AddCell(ttCel1.SetPadding(4));
                totalTable.AddCell(ttCel11.SetPadding(4));
                totalTable.AddCell(ttCel2.SetPadding(4));
                totalTable.AddCell(ttCel21.SetPadding(4));
                totalTable.AddCell(ttCel3.SetPadding(4));
                totalTable.AddCell(ttCel31.SetPadding(4));

                Cell cell3 = new Cell(1, 2).Add(customerTable);
                Cell cell4 = new Cell(1, 2).Add(productTable);
                Cell cell5 = new Cell(1, 2).Add(totalTable);

                Cell cell6 = new Cell(1, 2)
                    .Add(new Paragraph("ĐIỀU KIỆN BẢO HÀNH: Bảo hành theo tem dán trên sản phẩm hoặc vỏ hộp."))
                    .SetBold();

                table.AddCell(cell11.SetBorder(Border.NO_BORDER));
                table.AddCell(cell12.SetBorder(Border.NO_BORDER));
                table.AddCell(cell2.SetBorder(Border.NO_BORDER));
                table.AddCell(cell3.SetBorder(Border.NO_BORDER));
                table.AddCell(cell4.SetPaddingTop(12).SetBorder(Border.NO_BORDER));
                table.AddCell(cell5.SetBorder(Border.NO_BORDER));
                table.AddCell(cell6.SetBorder(Border.NO_BORDER));

                document.Add(table.SetFont(font).SetFontSize(11));
                document.Close();

                file = memoryStream.ToArray();
                memoryStream.Close();
            }

            return file;

        }
    }
}

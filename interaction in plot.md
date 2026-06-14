# CH1 - Tài liệu thiết kế tương tác theo map

## 0\. Mục đích tài liệu

Tài liệu này mô tả cách các đối tượng tương tác trong Chương 1 hoạt động khi đưa vào game. Người đọc tài liệu này cần hiểu được:

- Chương 1 có những map 3D nào.
- Trong mỗi map, đối tượng nào người chơi có thể bấm/tương tác.
- Đối tượng đó nằm ở đâu, trông như thế nào, dùng để làm gì.
- Khi bấm lần đầu thì xảy ra gì.
- Khi người chơi đã làm một việc nào đó rồi thì đối tượng đổi thoại/nội dung ra sao.
- Đối tượng nào chỉ dùng để chuyển cảnh, đối tượng nào có thoại, đối tượng nào mở UI.
- Đối tượng nào cần ghi nhận hành vi đánh giá người chơi.

Tài liệu này **không thay thế kịch bản chi tiết**. Nó là tài liệu trung gian để người làm Unity, dựng map, dựng UI và gắn hội thoại hiểu rõ cần triển khai tương tác thế nào.

## 1\. Cách hiểu về các loại tương tác

Trong Chương 1, không phải mọi thứ bấm được đều giống nhau. Có thể chia đơn giản như sau:

### 1.1. NPC có thoại

Là các nhân vật người chơi có thể lại gần và nói chuyện. Ví dụ:

- Bác bảo vệ.
- Chị Linh.
- Quân.
- Huy.
- Nam.
- Mai.

NPC có thể có:

- Thoại lần đầu.
- Thoại lặp sau khi đã nói xong.
- Thoại đổi theo mốc kịch bản.
- Thoại đổi theo route người chơi đã chọn.

### 1.2. Object chuyển cảnh

Là các điểm đưa người chơi sang map/cảnh khác. Ví dụ:

- Lối đi từ cổng vào khu tòa A.
- Lối lên phòng ở sảnh tòa A.
- Cửa phòng 308.
- Cửa lớp.

Các object này thường chỉ cần một câu chặn nếu người chơi chưa đủ điều kiện.

### 1.3. Object xem thông tin

Là các vật hoặc UI để người chơi đọc/xem thông tin. Ví dụ:

- Email nhận phòng.
- QR nhóm tòa.
- Slide bài tập nhóm.
- App mua hàng.

Loại này có thể không có thoại dài, nhưng có vai trò giúp người chơi kiểm tra thông tin.

### 1.4. UI tương tác chính

Là các màn hình điện thoại/laptop tạo ra lựa chọn hoặc hậu quả. Ví dụ:

- Bài đăng tài liệu miễn phí.
- Trang tài liệu giả.
- Cuộc gọi shipper.
- Ví điện tử.
- Chat riêng Nam.
- Chat nhóm BTL.
- Email học bổng.
- Form học bổng.
- SMS OTP.
- Màn hình tổng kết.

Đây là phần rất quan trọng của Chương 1. Nhiều tình huống lừa đảo không xảy ra bằng NPC đứng trước mặt người chơi, mà xảy ra qua điện thoại/laptop.

## 2\. Tổng quan 5 map chính

Chương 1 nên có 5 map 3D chính:

- **Cổng KTX / khu nhận hàng**
- **Sảnh tòa A / khu tiếp nhận tầng trệt**
- **Phòng 308 + hành lang**
- **Lớp học**
- **Hội trường sinh hoạt đầu khóa**

Các phần sau không dựng thành map 3D riêng, mà làm bằng UI/montage/cutscene ngắn:

- Email nhận phòng.
- Facebook/group tài liệu.
- Trang tài liệu giả.
- Chat phòng 308.
- Chat nhóm BTL.
- Chat riêng Nam.
- App mua hàng.
- Ví điện tử.
- Email học bổng.
- Form học bổng.
- SMS OTP.
- Màn hình tổng kết.

# MAP 1 - Cổng KTX / khu nhận hàng

## 1.1. Vai trò của map

Map này xuất hiện ở hai thời điểm:

- Đầu Chương 1: Minh vừa tới KTX, hỏi bác bảo vệ để biết đường vào khu tòa A.
- Sau tình huống shipper: Minh quay lại khu nhận hàng để kiểm tra đơn hàng thật.

Đây là **khu ngoài cổng KTX**, không phải sảnh tòa. Từ cổng vào sảnh tòa A phải đi thêm một đoạn đường nội khu/sân trước rồi rẽ vào tòa A.

## 1.2. Mô tả không gian

Map gồm:

- Cổng KTX chính.
- Chốt bảo vệ bên cạnh cổng.
- Biển BẢO VỆ.
- Bảng hướng dẫn vào khu tòa A, ví dụ: TÒA A / KHU TIẾP NHẬN SINH VIÊN MỚI →.
- Một đoạn sân/đường nội khu nhìn thấy phía sau cổng.
- Khu nhận hàng gần cổng hoặc ngoài cổng.
- Một số sinh viên/phụ huynh/NPC nền để tạo cảm giác ngày nhận phòng.

Không cần dựng toàn bộ đường từ cổng vào tòa A. Có thể dùng fade hoặc montage ngắn sau khi người chơi đi qua cổng.

## 1.3. Đối tượng tương tác trong map

### A. Điện thoại - email nhận phòng

**Loại:** UI xem thông tin.  
**Có bấm được:** có.  
**Dùng ở:** đầu M0.  
**Vai trò:** giúp người chơi kiểm tra lại thông tin nhận phòng.

**Mô tả:**

- Khi Minh đứng trước cổng, góc màn hình có icon điện thoại hoặc thông báo email.
- Tiêu đề thông báo: Thông tin nhận phòng KTX - Nguyễn Minh.

**Khi bấm lần đầu:**

- Mở email nhận phòng.
- Hiện các thông tin:
  - Tòa A.
  - Phòng 308.
  - Làm thủ tục tại khu tiếp nhận/sảnh tòa A.
  - Cần CCCD, email xác nhận, mã số sinh viên.

**Thoại/nhận xét của Minh:**

- Minh đọc lại thông tin chính:
  - "Tòa A, phòng 308."
  - "Làm thủ tục ở khu tiếp nhận."
  - "Vậy cứ hỏi bảo vệ rồi vào trong trước."

**Điều kiện đổi nội dung:**

- Không có nhánh phức tạp.
- Nếu đã đọc rồi, bấm lại chỉ mở lại email.

**Ghi nhận hành vi:**

- Có thể gọi TRACE_READ_RELEVANT_INFO khi người chơi mở email lần đầu.

**Ghi chú:**

- Đây không phải bẫy.
- Đây là tương tác dạy thói quen đọc nguồn chính thức.

### B. Bác bảo vệ

**Loại:** NPC có thoại.  
**Có bấm được:** có.  
**Dùng ở:** đầu M0.  
**Vai trò:** hướng dẫn Minh đi vào trong khu KTX và tới tòa A.

**Mô tả:**

- Bác bảo vệ đứng hoặc ngồi cạnh chốt bảo vệ.
- Mặc đồng phục bảo vệ.
- Gần đó có biển BẢO VỆ.
- Khi Minh lại gần, hiện prompt: Hỏi đường hoặc Nói chuyện.

**Khi bấm lần đầu:**

- Bác hỏi Minh có phải sinh viên nhận phòng không.
- Bác hỏi tòa nào.
- Bác hỏi có email xác nhận chưa.
- Bác chỉ Minh đi vào trong khu KTX, rẽ về phía tòa A/khu tiếp nhận.

**Thoại chính:**

- Bác bảo vệ: "Cháu nhận phòng hả?"
- Minh: "Dạ, con mới lên nhận phòng ạ."
- Bác bảo vệ: "Tòa nào?"
- Minh: "Dạ tòa A."
- Bác bảo vệ: "Có email xác nhận chưa?"
- Minh: "Dạ có."
- Bác bảo vệ: "Rồi, con đi vào trong, rẽ theo hướng tòa A. Tới sảnh tầng trệt có bàn hướng dẫn, người ta chỉ tiếp cho."
- Minh: "Dạ, con cảm ơn bác."

**Điều kiện sau tương tác:**

- Sau khi nói chuyện với bác, mở điểm chuyển cảnh đi vào khu tòa A.

**Nếu bấm lại:**

- Bác bảo vệ: "Con đi vào trong, rẽ hướng tòa A nha. Tới sảnh tầng trệt trước."

**Ghi nhận hành vi:**

- Không cần trace đánh giá. Đây là tương tác tiến trình.

**Ghi chú:**

- Không cần nhiều nhánh.
- Không biến bác bảo vệ thành người giảng về lừa đảo.

### C. Lối đi vào khu tòa A

**Loại:** điểm chuyển cảnh có điều kiện.  
**Có bấm được:** có, hoặc dùng trigger đi vào.  
**Dùng ở:** sau khi hỏi bác bảo vệ.  
**Vai trò:** chuyển từ map cổng sang sảnh tòa A.

**Mô tả:**

- Là lối đi phía sau cổng, hướng vào khu nội bộ KTX.
- Có thể đặt gần bảng chỉ hướng TÒA A / KHU TIẾP NHẬN SINH VIÊN MỚI →.

**Nếu chưa hỏi bác bảo vệ:**

- Minh: "Chắc mình nên hỏi bác bảo vệ trước."
- Không chuyển cảnh.

**Nếu đã hỏi bác bảo vệ:**

- Chạy fade/montage ngắn: Minh kéo vali đi qua sân nội khu, rẽ vào tòa A.
- Sau đó chuyển sang **Map 2 - Sảnh tòa A / khu tiếp nhận tầng trệt**.

**Ghi chú:**

- Không dựng thêm map đường nội khu nếu scope không cho phép.
- Quan trọng là trình bày đúng logic: cổng KTX và sảnh tòa A không nằm sát nhau.

### D. Khu nhận hàng

**Loại:** khu vực tương tác theo giai đoạn.  
**Có bấm được:** đầu game không cần; sau mốc shipper có thể dùng để kiểm tra.  
**Dùng ở:** sau M2.  
**Vai trò:** đối chiếu lời shipper giả với đơn hàng thật.

**Mô tả:**

- Nằm gần cổng hoặc ngoài cổng, nơi sinh viên thường nhận hàng.
- Có thể có vài shipper thật đứng giao hàng, vài gói hàng, vài sinh viên đứng chờ.
- Ở đầu game chỉ làm nền môi trường.

**Sau vụ shipper, nếu Minh đã chuyển tiền:**

- Minh xuống khu nhận hàng.
- Không thấy ai gọi tên mình.
- Người chơi mở app mua hàng.
- App cho thấy đơn vẫn đang vận chuyển, dự kiến ngày mai.
- Không có yêu cầu chuyển khoản ngoài app.

**Thoại khi phát hiện:**

- Minh: "Khoan…"
- Minh: "Đơn của mình chưa tới mà?"
- Minh: "Vậy hồi nãy là ai?"

**Sau vụ shipper, nếu Minh không chuyển tiền:**

- Có thể vẫn cho mở app để xác nhận rằng lựa chọn trì hoãn là hợp lý.
- App vẫn hiện đơn chưa tới hoặc còn đang vận chuyển.

**Ghi nhận hành vi:**

- Không cần trace riêng cho khu nhận hàng.
- Trace chính nằm ở lựa chọn trước đó: chuyển tiền hoặc trì hoãn.

**Ghi chú:**

- Không dựng shipper giả thành NPC ở đây.
- Shipper giả chỉ xuất hiện qua điện thoại trong Map 5.
- Shipper thật nếu có thì chỉ là NPC nền, không cần hội thoại riêng.

# MAP 2 - Sảnh tòa A / khu tiếp nhận tầng trệt

## 2.1. Vai trò của map

Map này dùng trong M0, sau khi Minh đã đi qua cổng, đi vào khu nội bộ KTX và rẽ vào tòa A. Đây là **sảnh tầng trệt của tòa A**, không phải khu ngay sau cổng.

Tại đây Minh làm thủ tục nhận phòng, gặp chị Linh, nhận thẻ phòng, quét QR nhóm tòa và đi lên phòng 308.

## 2.2. Mô tả không gian

Map gồm:

- Cửa vào tòa A.
- Sảnh tầng trệt.
- Bàn HỖ TRỢ SINH VIÊN MỚI.
- Chị Linh đứng gần bàn hướng dẫn.
- Bảng thông báo.
- QR nhóm tòa A.
- Cửa/vách Văn phòng tiếp nhận tòa A.
- Lối thang máy/cầu thang lên các tầng.
- Một số sinh viên mới đứng chờ, kéo vali hoặc cầm hồ sơ.

## 2.3. Đối tượng tương tác trong map

### A. Chị Linh

**Loại:** NPC có thoại theo tiến trình.  
**Có bấm được:** có.  
**Dùng ở:** M0.  
**Vai trò:** hướng dẫn Minh làm thủ tục và quét nhóm tòa.

**Mô tả:**

- Chị Linh đứng gần bàn hỗ trợ.
- Đeo thẻ Hỗ trợ sinh viên - Linh.
- Dáng đứng/biểu cảm thân thiện, giống người phụ trách hướng dẫn sinh viên mới.

**Khi bấm lần đầu:**

- Chị hỏi Minh nhận phòng hả.
- Minh nói tên, phòng, tòa.
- Chị hướng dẫn Minh vào văn phòng tiếp nhận để làm thủ tục.
- Chị nhắc làm xong quay lại quét QR nhóm tòa.

**Thoại chính:**

- Chị Linh: "Em nhận phòng hả?"
- Minh: "Dạ, em tên Nguyễn Minh, phòng 308."
- Chị Linh: "Tòa A đúng không em?"
- Minh: "Dạ đúng."
- Chị Linh: "Em vào văn phòng bên kia làm thủ tục nhận thẻ phòng nha. Chuẩn bị email xác nhận, CCCD với mã số sinh viên."
- Minh: "Dạ, em vào đó luôn hả chị?"
- Chị Linh: "Ừ. Làm xong quay lại đây, rồi em quét mã nhóm tòa để nhận thông báo."
- Minh: "Dạ, em cảm ơn chị."

**Nếu chưa làm thủ tục mà bấm lại:**

- Chị Linh: "Em vào văn phòng bên kia làm thủ tục nhận thẻ phòng trước nha."

**Nếu đã làm thủ tục nhưng chưa quét QR:**

- Chị Linh: "Em quét mã nhóm tòa ở bảng kia để nhận thông báo nha."

**Nếu người chơi hỏi QR có chính thức không:**

- Chị Linh xác nhận nhóm QR là nhóm thông báo chính thức của tòa A.
- Chị nói nhóm dùng để nhận thông báo về giờ giấc, nội quy, mất thẻ, báo sửa đồ.

**Nếu đã quét QR:**

- Chị Linh: "Rồi, em lên phòng 308 được rồi nha."

**Ghi nhận hành vi:**

- Nếu người chơi chủ động hỏi về QR: có thể gọi TRACE_ASK_PERSON.
- Nếu chỉ làm theo mạch chính, không cần trace riêng.

**Ghi chú:**

- Chị Linh là nguồn chính thức, nhưng không phải NPC giảng về scam.
- Thoại của chị nên gọn, đúng việc.

### B. Văn phòng tiếp nhận

**Loại:** object/cửa chạy thủ tục.  
**Có bấm được:** có.  
**Dùng ở:** M0.  
**Vai trò:** hoàn tất thủ tục nhận phòng.

**Mô tả:**

- Cửa hoặc vách kính ở sảnh tầng trệt.
- Có biển Văn phòng tiếp nhận tòa A.
- Không cần dựng đầy đủ bên trong.

**Nếu chưa nói với chị Linh:**

- Minh: "Chắc mình nên hỏi chị hướng dẫn trước."
- Không chạy thủ tục.

**Nếu đã nói với chị Linh:**

- Chạy cutscene/thủ tục ngắn:
  - kiểm tra email xác nhận,
  - đối chiếu CCCD,
  - ký tên,
  - nhận thẻ phòng,
  - nhận nội quy tòa A.

**Hiển thị sau thủ tục:**

- Nguyễn Minh - Tòa A - Phòng 308.
- Trạng thái: Đã nhận thẻ phòng.
- Vật phẩm nhận được: Thẻ phòng, nội quy tòa A.

**Nếu bấm lại:**

- Minh: "Mình làm thủ tục xong rồi."

**Ghi chú:**

- Không cần thêm NPC tiếp nhận riêng nếu không cần.
- Đây là tương tác chuyển trạng thái, không phải một map mới.

### C. QR nhóm tòa

**Loại:** object mở UI điện thoại.  
**Có bấm được:** có.  
**Dùng ở:** M0.  
**Vai trò:** đưa Minh vào nhóm thông báo chính thức của tòa A.

**Mô tả:**

- QR lớn trên bảng thông báo.
- Có dòng KTX Tòa A - Thông báo sinh viên.
- Có cảm giác đây là mã chính thức đặt trong sảnh tòa.

**Nếu chưa làm thủ tục:**

- Minh: "Mình nên làm thủ tục nhận phòng trước đã."
- Không cho tham gia nhóm.

**Nếu đã làm thủ tục:**

- Mở UI điện thoại.
- Hiện nhóm KTX Tòa A - Thông báo sinh viên.
- Có nút Tham gia nhóm.

**Sau khi tham gia nhóm:**

- Hiện tin ghim:
  - Chào mừng sinh viên Tòa A.
  - Vui lòng theo dõi thông báo chính thức tại nhóm này.
  - Không cung cấp mật khẩu, OTP hoặc thông tin tài khoản cá nhân cho bất kỳ ai.

**Thoại sau khi tham gia:**

- Minh: "Rồi, xong nhóm tòa."
- Minh: "Giờ lên phòng 308 thôi."

**Nếu bấm lại:**

- Minh: "Mình đã vào nhóm tòa rồi."

**Ghi chú:**

- QR nhóm tòa không phải bẫy.
- Đây là tương tác cho thấy kênh chính thức trông như thế nào.

### D. Lối lên phòng

**Loại:** chuyển cảnh có điều kiện.  
**Có bấm được:** có.  
**Dùng ở:** cuối M0 phần sảnh.  
**Vai trò:** chuyển từ sảnh tầng trệt lên hành lang/phòng 308.

**Mô tả:**

- Có thể là thang máy, cầu thang hoặc vùng trigger gần khu thang.

**Nếu chưa nhận thẻ phòng:**

- Minh: "Mình còn chưa làm thủ tục nhận phòng."
- Không chuyển cảnh.

**Nếu đã nhận thẻ phòng:**

- Chuyển sang Map 3 - Phòng 308 + hành lang.

**Ghi chú:**

- Không cần dựng quá chi tiết quá trình đi thang máy/cầu thang.
- Có thể dùng fade/chuyển cảnh.

# MAP 3 - Phòng 308 + hành lang

## 3.1. Vai trò của map

Đây là hub chính của Chương 1. Nhiều tình huống chính đều bắt đầu hoặc kết thúc ở đây:

- Minh vào phòng lần đầu, gặp Quân và Huy.
- Tìm tài liệu cho bài nhóm.
- Xử lý hậu quả link tài liệu giả.
- Nhận tin nhắn từ Nam.
- Hỏi Quân/Huy về Nam.
- Nhận email học bổng.
- Hỏi Quân/Huy về email học bổng.
- Tổng kết cuối chương.

## 3.2. Mô tả không gian

Map gồm:

- Một đoạn hành lang tầng 3.
- Cửa phòng 308.
- Bên trong phòng có 2 giường tầng, 3 bàn học, tủ cá nhân, quạt treo tường, thùng đồ.
- Khu giường của Quân.
- Khu bàn học/laptop của Huy.
- Bàn học của Minh với laptop và điện thoại.

Phòng 308 cần làm rõ là không gian sống chính của Minh trong chương.

## 3.3. Đối tượng tương tác trong map

### A. Cửa phòng 308

**Loại:** object chuyển cảnh/mở encounter.  
**Có bấm được:** có.  
**Dùng ở:** lần đầu Minh lên phòng.  
**Vai trò:** đưa người chơi vào phòng và gặp Quân/Huy.

**Mô tả:**

- Cửa phòng ở hành lang tầng 3.
- Có biển 308.
- Cửa có thể mở hé.

**Khi bấm lần đầu:**

- Minh bước vào phòng.
- Trigger thoại với Quân và Huy.

**Sau lần đầu:**

- Nếu game không cần free-roam, có thể không cần tương tác lặp.
- Nếu có free-roam, cửa dùng để ra/vào phòng.

**Ghi chú:**

- Cửa không cần thoại riêng.

### B. Quân

**Loại:** NPC có thoại theo giai đoạn.  
**Có bấm được:** có.  
**Dùng ở:** M0, M1, M3, M4, M5.  
**Vai trò:** bạn cùng phòng, tạo cảm giác đời sống, phản ứng tự nhiên với các sự kiện.

**Mô tả:**

- Quân ở khu giường tầng hoặc khu sinh hoạt trong phòng.
- Có thể đang dùng điện thoại/cắm sạc.
- Tông thoại thân, hơi giỡn, không nghiêm trọng hóa mọi thứ.

**M0 - lần đầu gặp:**

- Quân chào Minh.
- Trêu nhẹ vali/đồ đạc.
- Giúp người chơi thấy phòng 308 là môi trường sống chứ không chỉ là nơi nhận quest.

**M0 - nếu nói chuyện thêm:**

- Quân nhắc mấy chuyện thực tế: ổ khóa, thiếu gì thì hỏi, mới vào KTX thì từ từ quen.

**M1 - nếu hỏi về link tài liệu:**

- Quân xem bài đăng.
- Quân nói bài đăng nhìn hơi đáng nghi.
- Quân chỉ ra mấy điểm dễ thấy: tiêu đề đúng nhu cầu, miễn phí, bắt bấm link riêng.
- Sau đó Huy phân tích kỹ hơn.

**M1 - nếu Minh đã nhập mật khẩu:**

- Quân phản ứng ngắn, bất ngờ.
- Sau đó để Huy hướng dẫn xử lý.

**M3 - vụ Nam mượn tiền:**

- Nếu Minh hỏi Quân/Huy, Quân xem tin nhắn và nhận ra cụm tài khoản bạn tui nghe lạ.
- Quân không cần kết luận thay người chơi, chỉ làm người bạn cùng phòng phản ứng tự nhiên.

**M4 - vụ học bổng:**

- Quân phản ứng với số tiền 5 triệu và hạn 23:59.
- Có thể nói ngắn rằng nghe hấp dẫn nhưng hơi gấp.

**M5 - tổng kết:**

- Quân có vài câu chốt nhẹ, làm dịu không khí trước màn hình điểm.

**Thoại lặp mặc định:**

- "Thiếu gì thì hỏi nha ông."
- "Mới vô KTX thì từ từ quen."

**Trace:**

- Chỉ gọi trace khi người chơi chủ động hỏi Quân/Huy trong tình huống rủi ro.
- Không gọi trace cho nói chuyện đời thường.

**Ghi chú:**

- Quân không phải người giảng lý thuyết.
- Quân giúp cảnh bớt khô và bám đời sống sinh viên.

### C. Huy

**Loại:** NPC hỗ trợ/kiểm chứng.  
**Có bấm được:** có.  
**Dùng ở:** M0, M1, M3, M4, M5.  
**Vai trò:** người có kinh nghiệm hơn, đưa lời khuyên thực tế khi có tình huống rủi ro.

**Mô tả:**

- Huy ở bàn học/laptop.
- Cảm giác bận nhưng sẵn sàng trả lời ngắn gọn.
- Tông thoại thực tế, không dài dòng.

**M0 - lần đầu gặp:**

- Huy nhắc Minh giữ thẻ phòng, giấy tờ.
- Huy nhắc theo dõi nhóm tòa vì mấy hôm đầu dễ sót thông báo.

**M0 - nếu hỏi thêm:**

- Huy nói thông báo chính thức thì xem nhóm tòa.
- Còn các group ngoài như mua bán, tài liệu, việc làm thì phải coi kỹ nguồn.

**M1 - nếu hỏi về link tài liệu:**

- Huy bảo chỉ mở xem trước link, không đăng nhập.
- Huy chỉ ra link không phải nguồn trường/LMS.
- Huy gợi ý nguồn an toàn hơn: LMS, nhóm lớp, hỏi Mai.

**M1 - nếu Minh đã nhập mật khẩu:**

- Huy hướng dẫn đổi mật khẩu, đăng xuất thiết bị lạ, bật xác thực hai lớp.
- Huy nhắc tài liệu học không cần mật khẩu mạng xã hội.

**M3 - vụ Nam mượn tiền:**

- Huy khuyên gọi trực tiếp hoặc hỏi trong nhóm.
- Huy nhắc không chuyển qua tài khoản lạ.
- Huy nói nếu Nam cần thật thì xác minh qua kênh khác được.

**M4 - vụ học bổng:**

- Huy kiểm tra email/link.
- Huy nhận ra mail không giống kênh trường.
- Huy nhắc OTP không dùng để nhận học bổng.
- Huy nói những gì liên quan tiền, tài khoản, OTP thì kiểm tra trước.

**M4 - nếu Minh đã nhập OTP:**

- Huy hướng dẫn xử lý khẩn cấp:
  - khóa giao dịch,
  - gọi tổng đài ngân hàng,
  - không nhập thêm gì nữa,
  - lưu ảnh chụp email/form.

**M5 - tổng kết:**

- Huy chốt lại ngắn gọn rằng điều quan trọng là sau mỗi lần như vậy, Minh biết mình cần kiểm tra cái gì.

**Thoại lặp mặc định:**

- "Thông báo chính thức thì cứ xem trong nhóm tòa."
- "Mấy group ngoài thì coi kỹ nguồn trước khi làm theo."

**Trace:**

- Gọi trace khi người chơi hỏi Huy/Quân để kiểm chứng trong mốc rủi ro.
- Không gọi trace ở thoại đời thường.

**Ghi chú:**

- Huy là NPC giáo dục chính, nhưng lời thoại vẫn phải ngắn và tự nhiên.

### D. Bàn học / laptop của Minh

**Loại:** object mở UI.  
**Có bấm được:** có.  
**Dùng ở:** cuối M0, M1.  
**Vai trò:** mở tình huống link tài liệu.

**Mô tả:**

- Bàn học của Minh trong phòng 308.
- Có laptop, điện thoại, vở, giấy nháp.
- Đây là điểm để người chơi mở UI tìm tài liệu.

**Cuối M0:**

- Người chơi bấm laptop/điện thoại.
- Mở giao diện tìm tài liệu/Facebook.
- Hiện bài đăng tài liệu miễn phí.

**Bài đăng tài liệu hiển thị:**

- FULL TÀI LIỆU GIẢI TÍCH 1 + NHẬP MÔN NGÀNH.
- Drive tổng hợp cho tân sinh viên.
- Có đề cương, ví dụ mẫu, slide cũ.
- Nút NHẬN TÀI LIỆU.

**Lựa chọn từ bài đăng:**

- Đăng nhập tài khoản/mật khẩu theo yêu cầu.
- Hỏi trong phòng.
- Bỏ không tìm nữa.

**Nếu chọn nhận tài liệu:**

- Mở trang tài liệu giả.

**Nếu hỏi trong phòng:**

- Chuyển sang thoại với Quân/Huy.

**Nếu bỏ qua:**

- Không đi vào link lạ, nhưng thiếu chủ động tìm nguồn và ảnh hưởng phần bài nhóm.

**Trace:**

- Laptop/bài đăng không gọi trace riêng.
- Trace nằm ở hành vi sau đó.

**Ghi chú:**

- Laptop là object mở M1, cần ưu tiên implement.

### E. Trang tài liệu giả

**Loại:** UI form rủi ro.  
**Có bấm được:** có.  
**Dùng ở:** M1_R1.  
**Vai trò:** kiểm tra hành vi nhập tài khoản/mật khẩu vào nguồn chưa xác minh.

**Mô tả:**

- Trang web hiện sau khi bấm NHẬN TÀI LIỆU.
- Trông giống trang tổng hợp tài liệu sinh viên.
- Có nút đăng nhập bằng Facebook/Zalo.

**Nội dung hiển thị:**

- Kho tài liệu Tân sinh viên.
- FULL TÀI LIỆU GIẢI TÍCH 1 + NHẬP MÔN NGÀNH.
- Xác minh tài khoản sinh viên để nhận link Drive tổng hợp.
- Nút Tiếp tục bằng Facebook.
- Nút Tiếp tục bằng Zalo.

**Sau khi tiếp tục:**

- Hiện form nhập:
  - email/số điện thoại,
  - mật khẩu,
  - nút xác nhận.

**Nếu nhập thông tin:**

- Không tải được tài liệu.
- Điện thoại báo có lần đăng nhập mới.
- Chuyển sang đoạn xử lý hậu quả với Quân/Huy.

**Trace:**

- TRACE_SUBMIT_CREDENTIAL.

**Ghi chú:**

- UI phải có dấu hiệu lạ, nhưng không nên giả đến mức người chơi nhìn phát biết ngay.

### F. Điện thoại Minh

**Loại:** UI trung tâm.  
**Có bấm được:** có.  
**Dùng ở:** nhiều mốc.  
**Vai trò:** chứa nhiều tình huống qua chat/email/app.

**Mô tả:**

- Điện thoại là hệ UI chính của Chương 1.
- Nội dung điện thoại thay đổi theo mốc kịch bản.

**Các UI con cần có:**

- Chat phòng 308.
- Chat nhóm BTL.
- Chat riêng Nam.
- App mua hàng.
- Ví điện tử.
- Email học bổng.
- Form học bổng.
- SMS OTP.
- Recap cuối chương.

**Ghi chú:**

- Không làm điện thoại thành một object rỗng.
- Mỗi mốc cần bật đúng nội dung điện thoại tương ứng.

### G. Chat riêng Nam

**Loại:** UI chat + lựa chọn.  
**Có bấm được:** có.  
**Dùng ở:** M3.  
**Vai trò:** tình huống mạo danh người quen.

**Mô tả:**

- Chat riêng hiện tên Nam.
- Về thực chất, tài khoản Nam đang bị chiếm.
- Không cần model 3D cho Nam giả.

**Tin nhắn đầu:**

- Nam hỏi Minh còn thức không.
- Nam nói đang kẹt chuyện.
- Nam xin chuyển 250.000đ.
- Nam nói gấp, đang ngoài đường.
- Nam yêu cầu chuyển qua tài khoản bạn.

**Lựa chọn:**

- Chuyển tiền luôn.
- Hỏi anh em trong phòng.
- Chất vấn lại.

**Nếu chất vấn lại:**

- Minh hỏi đang ở đâu.
- Minh hỏi có gọi được không.
- Minh hỏi chi tiết liên quan bài tập.
- Minh hỏi vì sao không chuyển vào tài khoản Nam.
- Tài khoản Nam trả lời chung chung, né gọi, tiếp tục hối chuyển tiền.

**Sau nhánh chất vấn:**

- Mở lựa chọn tiếp:
  - vẫn chuyển,
  - hỏi Quân/Huy,
  - không chuyển và nhắn nhóm BTL.

**Trace:**

- Chất vấn lại: TRACE_DETECT_RED_FLAG.
- Chuyển tiền: TRACE_TRANSFER_MONEY.

**Ghi chú:**

- Giọng Nam giả phải khác Nam thật: gấp, né chi tiết, thúc chuyển tiền.

### H. Chat nhóm BTL

**Loại:** UI chat nhóm.  
**Có bấm được:** có.  
**Dùng ở:** M1, M3, M4.  
**Vai trò:** xác minh thông tin qua nhóm và cảnh báo người khác.

**M1 - tìm nguồn tài liệu:**

- Minh hỏi nhóm có nguồn tài liệu đáng tin hơn không.
- Mai gửi tài liệu/ghi chú.
- Nam gửi ví dụ mẫu.

**M3 - xác minh Nam:**

- Minh hỏi Nam thật có nhắn riêng không.
- Nam thật nói không.
- Nhóm nhận ra tài khoản Nam bị đăng nhập lạ.
- Nếu người chơi cảnh báo, nhóm/lớp được nhắc không chuyển tiền.

**M4 - hỏi học bổng:**

- Minh gửi email học bổng vào nhóm.
- Mai kiểm tra địa chỉ email, link, hạn gấp, form ngoài.
- Nhóm nhận ra email đáng nghi.
- Có thể cảnh báo nhóm lớp.

**Trace:**

- Hỏi người khác: TRACE_ASK_PERSON.
- Kiểm tra nguồn: TRACE_INSPECT_SOURCE nếu có hành vi kiểm tra rõ.
- Cảnh báo người khác: TRACE_WARN_OTHERS.

**Ghi chú:**

- Chat nhóm BTL là tương tác rất quan trọng, không phải UI phụ.

### I. Email học bổng

**Loại:** UI email + lựa chọn.  
**Có bấm được:** có.  
**Dùng ở:** M4.  
**Vai trò:** mở tình huống học bổng giả.

**Mô tả:**

- Email xuất hiện khi Minh đang có áp lực tiền bạc/chi phí sinh hoạt.
- Nội dung nhìn có vẻ liên quan tân sinh viên và hỗ trợ tài chính.

**Hiển thị:**

- Người gửi: <hocbong.tansinhvien@hotro-sv.com>.
- Tiêu đề: Học bổng Đồng hành Tân sinh viên 2025.
- Số tiền hỗ trợ: 5.000.000đ.
- Hạn xác nhận: 23:59 hôm nay.
- Nút XÁC NHẬN HỒ SƠ.

**Lựa chọn:**

- Làm theo form và nhập OTP.
- Hỏi nhóm BTL.
- Hỏi Quân/Huy.

**Trace:**

- Email tự thân không gọi trace.
- Trace nằm ở hành động sau đó.

**Ghi chú:**

- Email cần đủ thuyết phục nhưng có dấu hiệu nghi: mail không chính thức, hạn gấp, form ngoài, thông tin ngân hàng.

### J. Form học bổng / OTP

**Loại:** UI form rủi ro cao.  
**Có bấm được:** có.  
**Dùng ở:** M4_R1.  
**Vai trò:** route nguy hiểm nhất Chương 1.

**Form bước 1:**

- Họ tên.
- MSSV.
- Khoa/ngành.
- Khóa.
- Tòa KTX.
- Phòng KTX.
- Số điện thoại.

**Form bước 2:**

- Tên ngân hàng.
- Số tài khoản.
- Tên chủ tài khoản.
- Số điện thoại đăng ký ngân hàng.

**OTP:**

- SMS ngân hàng gửi mã OTP và cảnh báo không cung cấp mã này cho bất kỳ ai.
- Form lại yêu cầu nhập OTP để xác nhận tài khoản nhận học bổng.

**Nếu nhập OTP:**

- Hiện cảnh báo giao dịch bất thường.
- Chuyển sang đoạn xử lý hậu quả với Quân/Huy.

**Trace:**

- Điền thông tin tài khoản: TRACE_FILL_SENSITIVE_INFO.
- Nhập OTP: TRACE_SUBMIT_AUTH_CODE.

**Ghi chú:**

- Đây là đoạn cần làm rõ mâu thuẫn giữa SMS ngân hàng và form ngoài.

### K. Điện thoại recap / màn hình tổng kết

**Loại:** UI tổng kết.  
**Có bấm được:** có thể có.  
**Dùng ở:** M5.  
**Vai trò:** nhắc lại các dấu mốc và hiển thị feedback cuối chương.

**Hiển thị dấu mốc:**

- Bài đăng tài liệu miễn phí.
- Cuộc gọi shipper ở hội trường.
- Tin nhắn Nam mượn tiền.
- Email học bổng tân sinh viên.

**Hiển thị theo route:**

- Nếu từng nhập tài khoản: đã đổi mật khẩu, đã đăng xuất thiết bị lạ.
- Nếu mất tiền shipper: giao dịch -118.000đ.
- Nếu chuyển tiền cho Nam giả: giao dịch -250.000đ.
- Nếu nhập OTP: giao dịch online đã tạm khóa, đã lưu ảnh chụp email/form.
- Nếu xử lý an toàn: đã cảnh báo nhóm lớp, đã kiểm tra nguồn.

**Sau đó:**

- Chuyển sang màn hình tổng kết điểm.

**Trace:**

- M5 không gọi trace rủi ro mới.
- Chỉ tổng hợp trace/route trước đó.

**Ghi chú:**

- Không thêm scam mới ở M5.

# MAP 4 - Lớp học

## 4.1. Vai trò của map

Map lớp học dùng để Minh gặp Nam và Mai, lập nhóm bài tập, nhận yêu cầu thuyết trình và từ đó có lý do tìm tài liệu. Map này không cần quá lớn. Chỉ cần đủ để người chơi hiểu đây là buổi học đầu tiên.

## 4.2. Mô tả không gian

Map gồm:

- Cửa lớp, có thể ghi B2-304.
- Một số bàn ghế.
- Bảng/màn chiếu.
- Nam.
- Mai.
- Một vài sinh viên nền.

## 4.3. Đối tượng tương tác trong map

### A. Cửa lớp

**Loại:** trigger/chuyển cảnh.  
**Có bấm được:** có.  
**Dùng ở:** M0.  
**Vai trò:** đưa Minh vào lớp và mở cảnh gặp Nam.

**Tương tác:**

- Minh tìm phòng B2-304.
- Gặp Nam cũng đang tìm phòng.
- Hai người xác nhận cùng lớp.
- Bấm vào cửa/lối vào để vào lớp.

**Ghi chú:**

- Cửa lớp không cần nhiều trạng thái.
- Đây chỉ là trigger vào scene lớp học.

### B. Nam

**Loại:** NPC thoại tuyến tính.  
**Có bấm được:** có, hoặc xuất hiện trong cutscene.  
**Dùng ở:** M0.  
**Vai trò:** bạn nhóm BTL, tiền đề cho vụ mạo danh ở M3.

**Mô tả:**

- Sinh viên cùng lớp.
- Gặp Minh khi cả hai đang tìm phòng hoặc vào lớp.
- Nói chuyện bình thường, thân thiện.

**Tương tác chính:**

- Nam hỏi Minh có nhóm chưa.
- Nam rủ Minh ghép nhóm với Mai.
- Sau khi lập nhóm, Nam nhận phần ví dụ minh họa.

**Thoại lặp sau khi lập nhóm:**

- Nam: "Tối tui kiếm ví dụ minh họa."
- Nam: "Có gì không rõ thì nhắn nha."

**Ghi chú:**

- Nam thật phải nói bình thường.
- Sau này tài khoản Nam giả cần khác giọng để người chơi thấy có dấu hiệu lệch.

### C. Mai

**Loại:** NPC thoại tuyến tính.  
**Có bấm được:** có, hoặc xuất hiện trong cutscene.  
**Dùng ở:** M0.  
**Vai trò:** bạn nhóm BTL, người rõ việc và là nguồn xác minh sau này.

**Mô tả:**

- Sinh viên cùng lớp.
- Bình tĩnh, rõ việc, không quá lạnh.

**Tương tác chính:**

- Mai đồng ý lập nhóm với Minh và Nam.
- Mai phân việc:
  - Mai lo bố cục/lý thuyết chính.
  - Minh tìm tài liệu nền và phần mở đầu.
  - Nam tìm ví dụ minh họa.

**Thoại lặp sau khi lập nhóm:**

- Mai: "Cậu lấy phần mở đầu nha."
- Mai: "Có gì hỏi sớm để mình ráp bài."

**Ghi chú:**

- Mai về sau là nhân vật giúp xác minh link/tin nhắn/email trong nhóm BTL.

### D. Slide / bảng bài tập nhóm

**Loại:** object xem thông tin.  
**Có bấm được:** có thể có.  
**Dùng ở:** cuối buổi học đầu.  
**Vai trò:** tạo deadline và dẫn sang M1.

**Hiển thị:**

- BÀI TẬP NHÓM NHỎ - TUẦN 1.
- Nội dung: tóm tắt ý chính và trình bày ví dụ minh họa.
- Hình thức: thuyết trình ngắn nhóm 3 người.
- Thời lượng: 5-7 phút.
- Nhóm đầu tiên trình bày vào buổi sau.

**Tương tác:**

- Có thể tự hiện trong cutscene.
- Hoặc người chơi bấm xem bảng/slide.

**Ghi chú:**

- Slide chỉ cần truyền rõ deadline và nhiệm vụ.
- Không cần làm thành mini-game.

# MAP 5 - Hội trường sinh hoạt đầu khóa

## 5.1. Vai trò của map

Map này dùng cho mốc shipper. Minh đang ở hội trường, không tiện ra ngoài, điện thoại rung và có số lạ gọi yêu cầu chuyển tiền trước. Trọng tâm của map là áp lực quyết định, không phải khám phá không gian.

## 5.2. Mô tả không gian

Map gồm:

- Hàng ghế hội trường.
- Sinh viên ngồi xung quanh.
- Sân khấu phía trước.
- Banner SINH HOẠT ĐẦU KHÓA.
- Minh ngồi trong hàng ghế.
- Điện thoại rung.

## 5.3. Đối tượng tương tác trong map

### A. Điện thoại rung

**Loại:** trigger cuộc gọi.  
**Có bấm được:** có.  
**Dùng ở:** mở đầu M2.  
**Vai trò:** mở cuộc gọi shipper giả.

**Mô tả:**

- Điện thoại rung khi Minh đang ngồi trong hội trường.
- UI hiện cuộc gọi từ Số lạ hoặc Shipper - Giao hàng KTX.

**Khi bấm:**

- Minh nghe máy.
- Chuyển sang UI cuộc gọi shipper.

**Thoại đầu:**

- Minh: "Ai gọi giờ này vậy…"

**Ghi chú:**

- Đây là tương tác chính của map.
- Không cần cho người chơi đi lại trong hội trường.

### B. Cuộc gọi shipper giả

**Loại:** UI cuộc gọi + lựa chọn.  
**Có bấm được:** có.  
**Dùng ở:** M2.  
**Vai trò:** tình huống rủi ro shipper.

**Mô tả:**

- Không có model 3D.
- Chỉ có giọng nói/subtitle/UI cuộc gọi.
- Người gọi nói đúng tên Minh, đúng KTX, đúng số tiền 118.000đ.

**Nội dung chính:**

- Shipper nói có đơn giao tới KTX.
- Đơn cần thanh toán trước 118.000đ.
- Minh chuyển khoản thì shipper sẽ gửi bảo vệ.
- Người gọi hối vì còn nhiều đơn.

**Lựa chọn:**

- Chuyển tiền luôn.
- Hẹn lại hôm khác.
- Nhắn tin hỏi.

**Nếu chọn chuyển tiền:**

- Mở ví điện tử 118.000đ.

**Nếu chọn hẹn lại:**

- Minh từ chối chuyển khi chưa nhận hàng.
- Sau đó kiểm tra app mua hàng.

**Nếu chọn nhắn tin hỏi:**

- Mở chat phòng 308.
- Không ai trả lời kịp.
- Người chơi chọn lại giữa chuyển tiền và hẹn lại.

**Trace:**

- Chuyển tiền: TRACE_TRANSFER_MONEY.
- Hẹn lại: TRACE_DELAY_ACTION.
- Nhắn hỏi: TRACE_ASK_PERSON, tác động nhẹ.

**Ghi chú:**

- Không dựng shipper giả ngoài cổng.
- Việc chỉ nghe qua điện thoại là yếu tố quan trọng của tình huống.

### C. Ví điện tử 118.000đ

**Loại:** UI giao dịch.  
**Có bấm được:** có nếu chọn chuyển tiền.  
**Dùng ở:** M2_R1.  
**Vai trò:** hành động rủi ro chính của mốc shipper.

**Hiển thị:**

- Người nhận: TRAN VAN H....
- Số tiền: 118.000đ.
- Nội dung: Minh KTX A.
- Nút Xác nhận chuyển tiền.

**Nếu xác nhận:**

- Hiện Giao dịch thành công.
- Sau buổi sinh hoạt, Minh xuống khu nhận hàng và phát hiện đơn không khớp.

**Trace:**

- TRACE_TRANSFER_MONEY.

**Ghi chú:**

- Đây là quyết định có hậu quả rõ.
- Không chỉ làm thành ảnh minh họa.

### D. Chat phòng 308

**Loại:** UI chat.  
**Có bấm được:** có nếu chọn nhắn tin hỏi.  
**Dùng ở:** M2_TC_1.  
**Vai trò:** thể hiện hành vi hỏi người khác nhưng không có phản hồi kịp.

**Mô tả:**

- Màn hình có thể chia hai:
  - một bên là cuộc gọi số lạ,
  - một bên là chat Phòng 308.

**Tương tác:**

- Minh nhắn hỏi Quân/Huy có nên chuyển khoản cho shipper không.
- Tin nhắn hiện Đã gửi.
- Không ai trả lời ngay.
- Shipper tiếp tục hối.

**Sau đó:**

- Người chơi phải chọn lại:
  - chuyển tiền,
  - hẹn lại.

**Trace:**

- TRACE_ASK_PERSON, tác động nhẹ.

**Ghi chú:**

- Không để Quân/Huy trả lời kịp ở nhánh này, vì điểm của nhánh là người chơi vẫn phải tự quyết dưới áp lực.

### E. Lựa chọn hẹn lại / từ chối chuyển ngay

**Loại:** lựa chọn trong UI cuộc gọi.  
**Có bấm được:** có.  
**Dùng ở:** M2_R2.  
**Vai trò:** route kháng áp lực.

**Tương tác:**

- Minh nói đang ở hội trường, không ra được.
- Người gọi yêu cầu chuyển khoản trước.
- Minh từ chối chuyển khi chưa nhận hàng, hẹn giao lại sau.

**Sau đó:**

- Minh kiểm tra app mua hàng.
- App cho thấy đơn chưa tới và không có yêu cầu chuyển khoản ngoài app.

**Trace:**

- TRACE_DELAY_ACTION.

**Ghi chú:**

- Đây là lựa chọn đúng về mặt hành vi.
- Không cần làm thành object 3D riêng.

# Tổng kết nhanh cho triển khai

## Map 1 - Cổng KTX / khu nhận hàng

Đối tượng tương tác chính:

- Điện thoại/email nhận phòng.
- Bác bảo vệ.
- Lối đi vào khu tòa A.
- Khu nhận hàng.
- App mua hàng khi kiểm tra sau vụ shipper.

## Map 2 - Sảnh tòa A / khu tiếp nhận tầng trệt

Đối tượng tương tác chính:

- Chị Linh.
- Văn phòng tiếp nhận.
- QR nhóm tòa.
- Lối lên phòng.

## Map 3 - Phòng 308 + hành lang

Đối tượng tương tác chính:

- Cửa phòng 308.
- Quân.
- Huy.
- Bàn/laptop Minh.
- Điện thoại Minh.
- Trang tài liệu giả.
- Chat riêng Nam.
- Chat nhóm BTL.
- Email học bổng.
- Form học bổng / OTP.
- Điện thoại recap / màn hình tổng kết.

## Map 4 - Lớp học

Đối tượng tương tác chính:

- Cửa lớp.
- Nam.
- Mai.
- Slide/bảng bài tập nhóm.

## Map 5 - Hội trường sinh hoạt đầu khóa

Đối tượng tương tác chính:

- Điện thoại rung.
- Cuộc gọi shipper giả.
- Ví điện tử 118.000đ.
- Chat phòng 308.
- Lựa chọn hẹn lại / từ chối chuyển ngay.

# Lưu ý scope

- Chỉ những đối tượng trên cần logic tương tác rõ.
- Sinh viên nền, shipper thật nền, bàn ghế, giường, vali, thùng đồ, bảng trang trí… chỉ cần làm môi trường.
- Không dựng shipper giả 3D.
- Không dựng riêng văn phòng tiếp nhận, căn tin, cửa hàng, đường nội khu nếu không đủ thời gian.
- UI điện thoại/laptop là gameplay chính của Chương 1, cần ưu tiên hơn nhiều props phụ.

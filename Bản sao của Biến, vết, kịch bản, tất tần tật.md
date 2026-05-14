## **3.x Thiết kế cấu trúc kịch bản và quy ước mã hóa tuyến nội dung**

Để triển khai Chương 1 một cách thống nhất, đề tài sử dụng hệ quy ước riêng nhằm tổ chức kịch bản thành các đơn vị có thể quản lý, viết, kiểm tra và triển khai trong game. Hệ quy ước này được xây dựng dựa trên định hướng thiết kế đã trình bày ở Chương 2: game không sử dụng cây phân nhánh lớn, mà đi theo tuyến chính có các mốc lựa chọn cục bộ và điểm hội tụ. Nhờ đó, người chơi vẫn có thể đưa ra lựa chọn và nhận hệ quả tương ứng, trong khi nhóm phát triển vẫn kiểm soát được khối lượng nội dung trong phạm vi đồ án.

Trong báo cáo chính, phần này chỉ trình bày cấu trúc tổ chức, quy ước mã hóa và cách đọc luồng kịch bản. Toàn bộ nội dung kịch bản chi tiết, bao gồm hội thoại, mô tả cảnh, lựa chọn, cập nhật biến và chuyển cảnh cụ thể, được trình bày ở phần phụ lục để tránh làm phần thiết kế chính quá dài.

### **3.x.1 Phân cấp tổ chức kịch bản**

Kịch bản Chương 1 được tổ chức theo chuỗi:

**Chương → Mốc → Route → Đoạn kịch bản → Lựa chọn**

Trong đó, **Chương** là đơn vị nội dung lớn nhất. Trong phạm vi đồ án, Chương 1 đóng vai trò mở đầu, giúp người chơi làm quen với bối cảnh sinh viên, ký túc xá, nhóm bạn, các nhu cầu ban đầu và các tình huống lừa đảo được lồng ghép vào đời sống thường ngày.

**Mốc** là điểm tiến triển chính trong mạch chương. Mỗi mốc thường gắn với một sự kiện quan trọng, một tình huống rủi ro hoặc một điểm người chơi phải đưa ra lựa chọn. Ví dụ, Chương 1 có các mốc như tình huống link tài liệu miễn phí, tình huống shipper giả, tình huống tài khoản người quen nhắn mượn tiền và tình huống email học bổng giả.

**Route** là đường đi ngắn phát sinh từ lựa chọn của người chơi tại một mốc. Mỗi route có thể tạo ra hệ quả khác nhau, nhưng không kéo dài thành một tuyến truyện độc lập hoàn toàn. Sau khi xử lý hệ quả cục bộ, các route thường hội tụ lại để tiếp tục mạch chính.

**Đoạn kịch bản** là đơn vị triển khai nội dung cụ thể bên trong mốc hoặc route. Một đoạn kịch bản có thể bao gồm mô tả cảnh, lời kể, hội thoại, hành động, nội dung hiển thị trên UI, lựa chọn, cập nhật biến hoặc chuyển cảnh.

**Lựa chọn** là điểm người chơi can thiệp vào tình huống. Trong đề tài này, lựa chọn không chỉ dùng để rẽ nhánh câu chuyện, mà còn thể hiện cách người chơi đánh giá rủi ro và phản ứng trước lừa đảo.

### **3.x.2 Quy ước mã định danh kịch bản**

Để dễ quản lý, các đơn vị kịch bản được mã hóa theo cấu trúc thống nhất. Mã định danh giúp nhóm phát triển truy vết luồng nội dung, kiểm tra route, liên kết lựa chọn với hệ quả và đối chiếu với các file kịch bản trong phụ lục.

Bảng sau trình bày các thành phần mã chính:

| Ký hiệu | Ý nghĩa | Ví dụ |
| ----- | ----- | ----- |
| `CH` | Chương trong game | `CH1` là Chương 1 |
| `M` | Mốc nội dung chính trong chương | `CH1_M1` là mốc 1 của Chương 1 |
| `R` | Route phát sinh từ lựa chọn tại mốc | `CH1_M1_R2` là route 2 của mốc 1 |
| `R0` | Đoạn hội tụ của một mốc | `CH1_M1_R0` là đoạn hội tụ sau các route của mốc 1 |
| `BB` | Điểm bẻ nhánh, tức vị trí xuất hiện lựa chọn chính | `CH1_M1_R0_BB_1` |
| `TC` | Đoạn trung chuyển hoặc lựa chọn trung gian | `CH1_M2_TC_1` |
| `RESULT` | Kết quả hoặc màn đánh giá cuối chương | `CH1_RESULT_SAFE`, `CH1_RESULT_RISK` |

Cấu trúc mã cơ bản được viết như sau:

CH1\_Mx\_Ry

Trong đó:

CH1: Chương 1\.  
Mx: Mốc thứ x.  
Ry: Route thứ y trong mốc đó.  
R0: Đoạn hội tụ của mốc.

Ví dụ:

CH1\_M0\_R0  
CH1\_M1\_R1  
CH1\_M1\_R2  
CH1\_M1\_R3  
CH1\_M1\_R0  
CH1\_M2\_R1  
CH1\_M2\_R2  
CH1\_M2\_R0

Trong một số trường hợp, một lựa chọn không tạo ra route lớn riêng mà chỉ dẫn đến một đoạn trung gian trước khi quay về các route chính. Khi đó sử dụng ký hiệu `TC`.

Ví dụ:

CH1\_M2\_TC\_1

`TC` được hiểu là đoạn trung chuyển hoặc tạm chọn. Trường hợp này phù hợp với tình huống người chơi chọn “nhắn tin hỏi” nhưng chưa nhận được phản hồi kịp thời, sau đó vẫn phải quyết định tiếp là chuyển tiền hoặc hẹn lại.

### **3.x.3 Quy ước điểm bẻ nhánh BB**

`BB` là viết tắt của **điểm bẻ nhánh**, tức vị trí trong kịch bản nơi người chơi gặp lựa chọn chính. Mã `BB` được dùng khi cần xác định chính xác lựa chọn xuất hiện ở đoạn nào, nhất là khi một route hoặc một mốc có nhiều hơn một điểm lựa chọn.

Cấu trúc đề xuất:

CH1\_Mx\_Ry\_BB\_n

Trong đó:

CH1: Chương 1\.  
Mx: Mốc thứ x.  
Ry: Route chứa điểm lựa chọn.  
BB\_n: Điểm bẻ nhánh thứ n trong đoạn đó.

Ví dụ:

CH1\_M0\_R0\_BB\_1  
CH1\_M1\_R0\_BB\_1  
CH1\_M2\_R0\_BB\_1  
CH1\_M3\_R0\_BB\_1  
CH1\_M4\_R0\_BB\_1

Trong báo cáo, `BB` không nhất thiết phải tách thành một file riêng. Nó có thể là một mã nội bộ nằm trong file kịch bản để chỉ rõ tại vị trí đó người chơi được đưa ra lựa chọn. Ví dụ, file `CH1_M1_R0.md` có thể chứa điểm bẻ nhánh `CH1_M1_R0_BB_1`.

Ví dụ cách ghi trong kịch bản:

\[BB: CH1\_M1\_R0\_BB\_1\]

\[Lựa chọn\]  
A. Đăng nhập tài khoản để tải tài liệu.  
B. Hỏi bạn cùng phòng xem link này có ổn không.  
C. Bỏ qua vì thấy hơi đáng nghi.

Cách dùng `BB` giúp nhóm phát triển dễ đối chiếu giữa kịch bản, sơ đồ luồng và hệ thống xử lý lựa chọn trong game.

### **3.x.4 Quy ước route và đoạn hội tụ**

Mỗi lựa chọn chính tại một điểm bẻ nhánh có thể dẫn đến một route khác nhau. Route được đánh số theo thứ tự lựa chọn hoặc theo thứ tự xử lý trong kịch bản.

Ví dụ ở mốc `CH1_M1`, người chơi gặp bài đăng tài liệu miễn phí yêu cầu đăng nhập tài khoản mạng xã hội:

A → CH1\_M1\_R1: Người chơi đăng nhập tài khoản theo yêu cầu.  
B → CH1\_M1\_R2: Người chơi hỏi bạn cùng phòng hoặc người đáng tin.  
C → CH1\_M1\_R3: Người chơi bỏ qua.

Sau khi xử lý hệ quả của từng route, các route hội tụ về:

CH1\_M1\_R0

`R0` được dùng cho đoạn hội tụ sau các route. Đoạn hội tụ không có nghĩa là lựa chọn trước đó vô nghĩa. Trước khi hội tụ, mỗi route cần để lại hệ quả riêng, ví dụ thay đổi trạng thái nhân vật, mất tiền, tăng nghi ngờ, mở thêm thông tin, thay đổi thái độ của NPC hoặc cập nhật dữ liệu hành vi. Khi hội tụ, mạch truyện chính tiếp tục, nhưng hệ quả trước đó vẫn có thể được lưu lại để ảnh hưởng đến phản hồi cuối chương.

### **3.x.5 Quy ước tag trong nội dung kịch bản**

Để file kịch bản dễ đọc và dễ triển khai, nội dung trong từng đoạn được đánh dấu bằng các tag thống nhất. Các tag này giúp phân biệt đâu là mô tả bối cảnh, đâu là hội thoại, đâu là hành động gameplay, đâu là lựa chọn, đâu là cập nhật biến hoặc chuyển cảnh.

| Tag | Ý nghĩa |
| ----- | ----- |
| `[Cảnh]` | Mở một cảnh, đoạn hoặc tình huống mới |
| `[Kể chuyện]` | Mô tả bối cảnh, cảm xúc, suy nghĩ hoặc diễn biến không phải lời thoại trực tiếp |
| `[Hội thoại]` | Lời nói trực tiếp của nhân vật hoặc nội tâm được trình bày như lời thoại |
| `[Hành động]` | Hành động của nhân vật hoặc hành động gameplay người chơi cần thực hiện |
| `[Hiển thị]` | Nội dung được hiển thị trên UI, điện thoại, bảng thông báo, tin nhắn, email hoặc vật thể trong môi trường |
| `[Lựa chọn]` | Lựa chọn chính của người chơi, thường dẫn đến route hoặc hệ quả rõ ràng |
| `[Tuỳ chọn]` | Lựa chọn phụ, tương tác nhỏ, có thể bỏ qua hoặc không nhất thiết hiển thị như lựa chọn chính |
| `[Cập nhật]` | Thay đổi biến, trạng thái, flag, quan hệ, tiền, rủi ro hoặc dữ liệu hành vi |
| `[Chuyển cảnh]` | Chuyển sang địa điểm, thời gian, scene hoặc mốc khác |

Ví dụ:

\[Cảnh\] Minh đang ngồi trong phòng KTX sau buổi học đầu tiên.

\[Kể chuyện\] Cả nhóm bài tập vừa thống nhất rằng ngày mai phải có bản thuyết trình sơ bộ. Minh mở điện thoại, tìm thử tài liệu trong các nhóm sinh viên.

\[Hiển thị\] Trên màn hình điện thoại xuất hiện một bài đăng: “Tổng hợp full tài liệu môn đại cương – miễn phí cho tân sinh viên”.

\[Hành động\] Người chơi bấm xem bài đăng.

\[BB: CH1\_M1\_R0\_BB\_1\]

\[Lựa chọn\]  
A. Đăng nhập tài khoản theo yêu cầu để tải tài liệu.  
B. Hỏi bạn cùng phòng xem link này có ổn không.  
C. Bỏ qua vì thấy hơi đáng nghi.

### **3.x.6 Phân biệt lựa chọn chính và tùy chọn**

Trong kịch bản, cần phân biệt rõ `[Lựa chọn]` và `[Tuỳ chọn]`.

`[Lựa chọn]` là quyết định chính của người chơi, thường tạo route, hệ quả rõ ràng hoặc ảnh hưởng đến tiến trình mốc. Đây là những lựa chọn cần được thể hiện rõ trong game, vì chúng là điểm người chơi can thiệp trực tiếp vào tình huống.

Ví dụ:

\[Lựa chọn\]  
A. Chuyển tiền cho shipper.  
B. Hẹn lại hôm khác.  
C. Nhắn tin hỏi lại người bán.

`[Tuỳ chọn]` là tương tác phụ, không bắt buộc hoặc không nhất thiết hiển thị như lựa chọn chính. Tuy nhiên, tùy chọn vẫn có thể ảnh hưởng nhẹ đến thông tin, biến, hội thoại hoặc cách người chơi hiểu tình huống.

Ví dụ:

\[Tuỳ chọn\] Kiểm tra tên người đăng bài trong nhóm.  
\[Tuỳ chọn\] Xem bình luận dưới bài đăng.  
\[Tuỳ chọn\] Hỏi Quân xem đã từng thấy nhóm này chưa.

Việc tách `[Lựa chọn]` và `[Tuỳ chọn]` giúp kịch bản không bị quá tải. Những quyết định lớn được đặt thành lựa chọn chính, còn những hành vi nhỏ như kiểm tra, quan sát, hỏi thêm hoặc xem thông tin phụ được đặt thành tùy chọn.

### **3.x.7 Quy ước cập nhật trạng thái và hành vi**

Tag `[Cập nhật]` dùng để ghi nhận thay đổi sau hành động hoặc lựa chọn của người chơi. Nội dung cập nhật có thể là biến trạng thái trong game, biến quan hệ, biến tiền, flag sự kiện hoặc dữ liệu hành vi phục vụ đánh giá.

Ví dụ:

\[Cập nhật\] money \-= 50000  
\[Cập nhật\] flag\_m1\_clicked\_fake\_link \= true  
\[Cập nhật\] trust\_roommate \+= 1  
\[Cập nhật\] trace\_check\_source \+= 1  
\[Cập nhật\] trace\_shared\_sensitive\_info \+= 1

Các dòng `[Cập nhật]` không nhất thiết hiển thị cho người chơi. Đây là thông tin dành cho nhóm thiết kế và triển khai, giúp xác định hành động trong kịch bản ảnh hưởng đến hệ thống như thế nào.

Trong báo cáo chính, phần này chỉ trình bày nguyên tắc cập nhật. Danh sách biến cụ thể, giá trị cộng/trừ, điều kiện mở lựa chọn và cách tính kết quả cuối chương sẽ được trình bày ở mục thiết kế hệ thống đánh giá hoặc phụ lục liên quan.

### **3.x.8 Quy ước đặt tên file kịch bản**

Các file kịch bản chi tiết nên được đặt tên theo mã mốc hoặc route để dễ quản lý. Mỗi file `.md` nên tương ứng với một mốc, một route hoặc một đoạn nội dung đủ rõ để triển khai.

Ví dụ:

CH1\_M0\_R0.md  
CH1\_M1\_R1.md  
CH1\_M1\_R2.md  
CH1\_M1\_R3.md  
CH1\_M1\_R0.md  
CH1\_M2\_R1.md  
CH1\_M2\_R2.md  
CH1\_M2\_TC\_1.md  
CH1\_M2\_R0.md

Cách đặt tên này giúp nhóm dễ biết file đang thuộc chương nào, mốc nào và route nào. Khi cần kiểm tra luồng, nhóm chỉ cần đối chiếu sơ đồ kịch bản với tên file tương ứng.

Nếu một file chứa nhiều điểm lựa chọn nhỏ, các điểm đó có thể được đánh dấu bằng mã `BB` bên trong file thay vì tách thành file riêng. Điều này giúp giảm số lượng file nhưng vẫn giữ được khả năng truy vết lựa chọn.

### **3.x.9 Luồng tổng quát Chương 1 theo hệ mã**

Theo cấu trúc hiện tại, Chương 1 được tổ chức theo tuyến chính có các mốc lựa chọn cục bộ và hội tụ như sau:

CH1\_M0  
→ CH1\_M0\_R0  
→ CH1\_M1  
  ├─ A → CH1\_M1\_R1 ┐  
  ├─ B → CH1\_M1\_R2 ├→ CH1\_M1\_R0  
  └─ C → CH1\_M1\_R3 ┘  
→ CH1\_M2  
  ├─ A → CH1\_M2\_R1 ┐  
  ├─ B → CH1\_M2\_R2 ├→ CH1\_M2\_R0  
  └─ C → CH1\_M2\_TC\_1  
            ├─ A → CH1\_M2\_R1  
            └─ B → CH1\_M2\_R2  
→ CH1\_M3  
  ├─ A → CH1\_M3\_R1 ┐  
  ├─ B → CH1\_M3\_R2 ├→ CH1\_M3\_R0  
  └─ C → CH1\_M3\_TC\_1  
            ├─ A → CH1\_M3\_R1  
            ├─ B → CH1\_M3\_R2  
            └─ C → CH1\_M3\_R0  
→ CH1\_M4  
  ├─ A → CH1\_M4\_R1 ┐  
  ├─ B → CH1\_M4\_R2 ├→ CH1\_M4\_R0  
  └─ C → CH1\_M4\_R3 ┘  
→ CH1\_M5  
→ CH1\_M5\_R0  
→ CH1\_RESULT\_\<TYPE\>

Trong luồng này, `CH1_M0` là phần mở đầu và dẫn nhập. Từ `CH1_M1` trở đi, mỗi mốc chính tương ứng với một tình huống rủi ro hoặc một cụm lựa chọn quan trọng. Sau mỗi mốc, các route xử lý hệ quả cục bộ rồi hội tụ về `R0` để tiếp tục mạch chính.

### **3.x.10 Ghi chú về phụ lục kịch bản**

Do kịch bản Chương 1 có nhiều đoạn hội thoại, mô tả, lựa chọn, cập nhật biến và chuyển cảnh, báo cáo chính không trình bày toàn bộ nội dung kịch bản chi tiết trong phần thân bài. Thay vào đó, phần thân bài chỉ trình bày cấu trúc tổ chức, quy ước mã hóa, sơ đồ luồng và bảng tóm tắt các mốc chính.

Toàn bộ kịch bản chi tiết của Chương 1 được đưa vào phần phụ lục. Các phụ lục này bao gồm nội dung từng file kịch bản `.md`, các đoạn route, các điểm bẻ nhánh, các lựa chọn, các cập nhật trạng thái và các đoạn hội tụ tương ứng.


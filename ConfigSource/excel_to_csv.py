import os
import pandas as pd

# Excel 源文件夹（绝对路径）
input_folder = r"D:\Driftlands\ConfigSource\Config_1"

# CSV 输出文件夹（绝对路径）
output_folder = r"D:\Driftlands\Assets\Configs"

# 如果输出文件夹不存在，自动创建
if not os.path.exists(output_folder):
    os.makedirs(output_folder)

# 注册编码支持（读取 Excel 时需要）
import sys
if sys.version_info < (3, 7):
    import importlib
    importlib.reload(sys)
import locale
import codecs

for file_name in os.listdir(input_folder):
    if file_name.endswith(".xlsx") or file_name.endswith(".xls"):
        excel_path = os.path.join(input_folder, file_name)
        csv_name = os.path.splitext(file_name)[0] + ".csv"
        csv_path = os.path.join(output_folder, csv_name)

        # 读取 Excel，第一个 Sheet
        df = pd.read_excel(excel_path, sheet_name=0)

        # 保存成 CSV，utf-8-sig 解决 Excel 打开乱码问题
        df.to_csv(csv_path, index=False, encoding="utf-8-sig")

        print(f"✔ 转换成功: {file_name} → {csv_name}")

print("全部转换完成！")
input("按任意键退出...")

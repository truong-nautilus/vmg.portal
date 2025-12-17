import pymssql
import os
import glob
import sys

# Configuration
HOST = "217.216.36.189"
USER = "sa"
PASS = "cdP7mSFrujsy8cYy5XK9Wep"
DB = "VMG.BillingDB"
SP_DIR = "Infrastructure/Persistence/StoredProcedures/VMG.BillingDB"

def migrate():
    print(f"--- Migrating SPs for {DB} ---")
    
    script_dir = os.path.dirname(os.path.abspath(__file__))
    sp_path = os.path.join(script_dir, SP_DIR)
    
    if not os.path.exists(sp_path):
        print(f"Error: Directory {sp_path} does not exist.")
        return

    try:
        conn = pymssql.connect(server=HOST, user=USER, password=PASS, database=DB, autocommit=True)
        cursor = conn.cursor()

        files = glob.glob(os.path.join(sp_path, "*.sql"))
        print(f"Found {len(files)} SQL files.")

        for filepath in files:
            filename = os.path.basename(filepath)
            print(f"Applying {filename}...")
            try:
                with open(filepath, "r", encoding="utf-8") as f:
                    sql_content = f.read()

                sp_name = os.path.splitext(filename)[0]
                
                # Drop and Re-create
#                 cursor.execute(f"IF OBJECT_ID('{sp_name}', 'P') IS NOT NULL DROP PROCEDURE [{sp_name}]")
                cursor.execute(sql_content)
                print(f"Updated {sp_name}")
                
            except Exception as e:
                print(f"Error migrating {filename}: {e}")

        conn.close()
        print("Migration complete.")
    except Exception as e:
        print(f"Error connecting to DB: {e}")

if __name__ == "__main__":
    migrate()

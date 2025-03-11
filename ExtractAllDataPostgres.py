import os
import re
from datetime import datetime
import requests
from bs4 import BeautifulSoup
import pandas as pd
import psycopg2
from psycopg2 import sql
from psycopg2.extras import execute_batch
from dotenv import load_dotenv
DATABASE_URL = os.getenv('DATABASE_URL')


# Configuration de la connexion SQL Server
# conn_str = (
#     r"DRIVER={ODBC Driver 17 for SQL Server};"
#     r"SERVER=(localdb)\MSSQLLocalDB;"
#     r"DATABASE=ClassementDB;"
#     r"Trusted_Connection=yes;"
# )

conn_str = {
    "host": "dpg-cv0sc5tsvqrc738v8s60-a",
    "database": "classement_db",
    "user": "classement_db_user",
    "password": "SRWO6rPlLyRgLRmp3tyKBHIC04GPZ0EY",
    "port": "5432",
    "sslmode": "require"
}

# conn_str = {
#     "host": "dpg-cv0gndaj1k6c73e8vo9g-a.frankfurt-postgres.render.com",
#     "database": "classement_db",
#     "user": "classement_db_user",
#     "password": "SRWO6rPlLyRgLRmp3tyKBHIC04GPZ0EY",
#     "port": "5432",
#     "sslmode": "require"
# }


ENGIE_TEAM_MAPPING = {
    '91': 'C2E ENGIE METZ 3',
    '92': 'C2E ENGIE Freyming',
    '111': 'C2E ENGIE Nancy',
    '117': 'C2E ENGIE METZ 2',
}

COLUMN_MAPPING = {
    'Engie': {
        'Date': r'Date',
        'Num_contrat': r'Contrat',
        'Vendeur': r'Commercial',
        'Status': r'Statut',
        'Client': r'Client',
        'Type': r'Type'
    },
    'Ohm': {
        'Date': r'Date',
        'Num_contrat': r'Num Contrat',
        'Vendeur': r'Commercial',
        'Status': r'Statut',
        'Client': r'Client'
    },
    'Bouygues': {
        'Date': r'signature',
        'Num_contrat': r'num_contrat',
        'Vendeur': r'vendeur',
        'Status': r'etat_commande',
        'Client': r'nom client',
        'Produite': r'produit',
        'Equipe': r'equipe'
    }
}

# Configuration du site
login_url = "https://myofficebyc2e.com/index.php"
username = "jessy"
password = "MyOffice118"
session = requests.Session()

def authenticate():
    """Authentification sur le site"""
    login_payload = {
        "username": username,
        "password": password,
        "login": "Login"
    }
    
    try:
        response = session.post(login_url, data=login_payload)
        response.raise_for_status()
        print("Authentification réussie")
    except Exception as e:
        print(f"Échec de l'authentification: {str(e)}")
        exit()

def map_columns(df, table_name):
    """Mappage dynamique des colonnes"""
    mapped_cols = {}
    for target, pattern in COLUMN_MAPPING[table_name].items():
        match = next((col for col in df.columns if re.search(pattern, col, re.IGNORECASE)), None)
        if match:
            mapped_cols[match] = target
    return df.rename(columns=mapped_cols)

def clean_data(df, table_name, equipe_name=None):
    """Nettoyage et préparation des données"""
    df = map_columns(df, table_name)
    
    required_columns = ['Date', 'Num_contrat', 'Vendeur', 'Status', 'Client']
    missing = [col for col in required_columns if col not in df.columns]
    
    if missing:
        print(f"Colonnes manquantes: {missing}")
        return pd.DataFrame()
    
    # Conversion des dates
    df['Date'] = df['Date'].apply(lambda x: convert_date(x, table_name))
    
    # Filtrage des dates
    if df['Date'].notna().any():
        df = df[df['Date'].notna()]
    else:
        print("Aucune date valide trouvée")
        return pd.DataFrame()

    # Nettoyage des numéros de contrat
    df['Num_contrat'] = df['Num_contrat'].astype(str).str.strip().str.replace(r'\W+', '', regex=True)
    
    # Gestion des équipes
    if table_name == 'Engie':
        df['Equipe'] = equipe_name
    elif table_name == 'Ohm':
        df['Equipe'] = 'DELTA BOUYGUES VALENCIENNES'
    elif table_name == 'Bouygues':
        df['Produite'] = df.get('Produite', '')
        df['Equipe'] = df.get('Equipe', '')
    
    # Sélection des colonnes
    columns = required_columns.copy()
    if table_name == 'Engie':
        columns += ['Type', 'Equipe']
    elif table_name == 'Bouygues':
        columns += ['Produite', 'Equipe']
    elif table_name == 'Ohm':
        columns += ['Equipe']
    
    return df[columns].drop_duplicates('Num_contrat')

def convert_date(date_val, table_name):
    try:
        if table_name == 'Engie':
            return pd.to_datetime(date_val.strip(), format='%Y.%m.%d %H:%M', errors='coerce')
        elif table_name == 'Ohm':
            return pd.to_datetime(date_val.strip(), format='%d/%m/%Y', dayfirst=True, errors='coerce')
        elif table_name == 'Bouygues':
            return pd.to_datetime(date_val.strip(), errors='coerce')
    except Exception as e:
        print(f"Erreur conversion date: {str(e)}")
        return pd.NaT

def insert_data(df, table_name):
    """Insertion optimisée dans PostgreSQL"""
    if df.empty:
        print("Aucune donnée à insérer")
        return

    conn = None  # Initialisation explicite
    try:
        # conn = psycopg2.connect(**conn_str, client_encoding='UTF8')
        conn = psycopg2.connect(dsn=os.getenv('DATABASE_URL'), sslmode='require', client_encoding='UTF8')
        cursor = conn.cursor()
        cursor.execute("SHOW client_encoding;")
        print("Encodage PostgreSQL:", cursor.fetchone())
        # Préparation des colonnes
        cols = ['Date', 'Num_contrat', 'Vendeur', 'Status', 'Client', 'Equipe']
        if table_name == 'Engie':
            cols.append('Type')
        elif table_name == 'Bouygues':
            cols.append('Produite')
        

        # Requête UPSERT
        # query = sql.SQL("""
        #     INSERT INTO c2e.{table} ({columns})
        #     VALUES ({values})
        #     ON CONFLICT ({conflict}) DO NOTHING
        # """).format(
        #     table=sql.Identifier(table_name),
        #     columns=sql.SQL(', ').join(map(sql.Identifier, cols)),
        #     values=sql.SQL(', ').join([sql.Placeholder()] * len(cols)),
        #     conflict=sql.Identifier('Num_contrat')
        # )


        if table_name in ['Engie', 'Ohm']:
            # Conflit sur Num_contrat uniquement
            conflict_column = sql.Identifier('Num_contrat')
        else:
            # Pour les autres tables (ex: Engie), conflit sur Num_contrat + Status
            conflict_column = sql.SQL(', ').join(map(sql.Identifier, ['Num_contrat', 'Status']))

            # Requête UPSERT
        query = sql.SQL("""
            INSERT INTO c2e.{table} ({columns})
            VALUES ({values})
            ON CONFLICT ({conflict}) DO NOTHING
        """).format(
            table=sql.Identifier(table_name),
            columns=sql.SQL(', ').join(map(sql.Identifier, cols)),
            values=sql.SQL(', ').join([sql.Placeholder()] * len(cols)),
            conflict=conflict_column 
            )

        # conflict_columns = ['Num_contrat', 'Status']

        # query = sql.SQL("""
        #     INSERT INTO c2e.{table} ({columns})
        #     VALUES ({values})
        #     ON CONFLICT ({conflict}) DO NOTHING
        # """).format(
        #     table=sql.Identifier(table_name),
        #     columns=sql.SQL(', ').join(map(sql.Identifier, cols)),
        #     values=sql.SQL(', ').join([sql.Placeholder()] * len(cols)),
        #     conflict=sql.SQL(', ').join(map(sql.Identifier, conflict_columns))
        # )
        
        # Conversion des données
        df = df.dropna(subset=['Num_contrat', 'Date'])
        df['Date'] = df['Date'].dt.strftime('%Y-%m-%d %H:%M:%S')
        # data = [tuple(row[col] for col in cols) for _, row in df.iterrows()]
        
        data = [tuple(str(row[col]).encode('utf-8', 'ignore').decode('utf-8') if isinstance(row[col], str) else row[col] for col in cols) for _, row in df.iterrows()]

        # Exécution
        execute_batch(cursor, query, data)
        conn.commit()
        print(f"{cursor.rowcount} lignes insérées ({table_name})")

    except Exception as e:
        print(f"ERREUR ({table_name}): {str(e)}")
        if conn:  # Vérification cruciale ici
            conn.rollback()
    finally:
        if conn:  # Fermeture sécurisée
            cursor.close()
            conn.close()

def process_data(post_url, payload, headers, excel_name, table_name, equipe_name=None):
    """Processus complet de collecte et traitement"""
    try:
        response = session.post(post_url, data=payload, headers=headers)
        response.raise_for_status()
        
        soup = BeautifulSoup(response.text, 'html.parser')
        table = soup.find('table', class_='table')  # Ajouter la classe spécifique si nécessaire
        
        if not table:
            print("Aucun tableau trouvé - vérifier la réponse HTTP")
            with open(f'debug_{table_name}.html', 'w', encoding='utf-8') as f:
                f.write(response.text)
            return
            
        headers_html = [th.get_text(strip=True) for th in table.find_all('th')]
        rows = []
        for tr in table.find_all('tr')[1:]:
            cells = [td.get_text(strip=True) for td in tr.find_all('td')]
            if len(cells) == len(headers_html):
                rows.append(cells)
        
        df = pd.DataFrame(rows, columns=headers_html)
        df.to_excel(excel_name, index=False)
        print(f"Fichier Excel généré: {excel_name}")
        
        df_clean = clean_data(df, table_name, equipe_name=equipe_name)
        
        if not df_clean.empty:
            insert_data(df_clean, table_name)
        else:
            print("Aucune donnée valide après nettoyage")
            
    except Exception as e:
        print(f"Erreur de traitement: {str(e)}")

def process_engie_data():
    """Traitement spécifique pour Engie"""
    for team_id, team_name in ENGIE_TEAM_MAPPING.items():
        print(f"\nTraitement équipe {team_name}...")
        process_data(
            post_url="https://myofficebyc2e.com/ventes_engie.php",
            payload={
                "recherche": "true",
                "jj1": "01", "mm1": "03", "aa1": "2025",
                "jj2": datetime.now().strftime("%d"),
                "mm2": datetime.now().strftime("%m"),
                "aa2": datetime.now().strftime("%Y"),
                "entite[]": "2",
                "equipe[]": [team_id],
                "statut[]": [
                    "70_CONTRAT_SIGNE_VALIDE",
                    "75_CONTRAT_SIGNE_VALIDE_AVEC_OPTION",
                    "76_CONTRAT_SIGNE_VALIDE_AVEC_OPTION_LG40"
                ]
            },
            headers={
                "Content-Type": "application/x-www-form-urlencoded",
                "Referer": "https://myofficebyc2e.com/ventes_engie.php",
                "X-Requested-With": "XMLHttpRequest"
            },
            excel_name=f"engie_{team_name.replace(' ', '_')}.xlsx",
            table_name="Engie",
            equipe_name=team_name
        )

# Exécution principale
authenticate()

# Traitement Engie par équipe
process_engie_data()

# Traitement Ohm
process_data(
    post_url="https://myofficebyc2e.com/ventes_ohm.php",
    payload={
        "recherche": "true",
        "jj1": "01", "mm1": "03", "aa1": datetime.now().strftime("%Y"),
        "jj2": datetime.now().strftime("%d"),
        "mm2": datetime.now().strftime("%m"),
        "aa2": datetime.now().strftime("%Y"),
        "statut[]": ["Accepted", "effective"]
    },
    headers={
        "Content-Type": "application/x-www-form-urlencoded",
        "Referer": "https://myofficebyc2e.com/ventes_ohm.php"
    },
    excel_name="ohm_data.xlsx",
    table_name="Ohm"
)

# Traitement Bouygues
process_data(
    post_url="https://myofficebyc2e.com/journal_bouygues.php",
    payload={
        "recherche": "true",
        "jj1": "01", "mm1": "03", "aa1": datetime.now().strftime("%Y"),
        "entite[]": ["2"]
    },
    headers={
        "Content-Type": "application/x-www-form-urlencoded",
        "Referer": "https://myofficebyc2e.com/journal_bouygues.php"
    },
    excel_name="bouygues_data.xlsx",
    table_name="Bouygues"
)



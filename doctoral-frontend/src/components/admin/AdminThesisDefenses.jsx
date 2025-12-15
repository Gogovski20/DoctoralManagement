import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { studentService } from "../../api/studentService";

export default function AdminThesisDefenses() {
  const [defenses, setDefenses] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const data = await studentService.getAllDefenses();
      setDefenses(data);
    } catch (err) {
      console.error("Failed to load defenses:", err);
    } finally {
      setLoading(false);
    }
  };

  const completeDefense = async (id) => {
    try {
      await studentService.completeDefense(id);
      loadData();
    } catch (err) {
      console.error("Failed to complete defense:", err);
    }
  };

  const finalizeDefense = async (id) => {
    try {
      await studentService.finalizeDefense(id);
      loadData();
    } catch (err) {
      console.error("Failed to finalize defense:", err);
    }
  };

  if (loading) return <p style={{ padding: "2rem" }}>Loading...</p>;

  return (
    <div style={{ padding: "2rem" }}>
      <h1 style={{ fontSize: "2rem", fontWeight: "bold", marginBottom: "1rem" }}>
        Thesis Defenses
      </h1>

      {/* Schedule New Defense */}
      <Link
        to="/admin/thesis-defenses/schedule"
        style={{
          display: "inline-block",
          marginBottom: "1.5rem",
          padding: "0.7rem 1.2rem",
          background: "#0d9488",
          color: "white",
          borderRadius: "0.5rem",
          textDecoration: "none",
        }}
      >
        ➕ Schedule New Thesis Defense
      </Link>

      <table width="100%" style={{ borderCollapse: "collapse", background: "white" }}>
        <thead>
          <tr style={{ background: "#f3f4f6", textAlign: "left" }}>
            <th style={{ padding: "10px" }}>Student</th>
            <th style={{ padding: "10px" }}>Project</th>
            <th style={{ padding: "10px" }}>Scheduled At</th>
            <th style={{ padding: "10px" }}>Room</th>
            <th style={{ padding: "10px" }}>Status</th>
            <th style={{ padding: "10px" }}>Actions</th>
          </tr>
        </thead>

        <tbody>
          {defenses.map((def) => (
            <tr key={def.id} style={{ borderBottom: "1px solid #e5e7eb" }}>
              <td style={{ padding: "10px" }}>{def.studentName}</td>
              <td style={{ padding: "10px" }}>{def.projectTitle}</td>
              <td style={{ padding: "10px" }}>
                {new Date(def.scheduledAt).toLocaleString()}
              </td>
              <td style={{ padding: "10px" }}>{def.room}</td>
              <td style={{ padding: "10px" }}>{def.status}</td>

              <td style={{ padding: "10px" }}>
                {def.status === "Scheduled" && (
                  <button
                    onClick={() => completeDefense(def.id)}
                    style={{
                      padding: "6px 12px",
                      background: "#2563eb",
                      color: "white",
                      borderRadius: "4px",
                      border: "none",
                      marginRight: "8px",
                      cursor: "pointer",
                    }}
                  >
                    Complete
                  </button>
                )}

                {def.status === "Completed" && (
                  <button
                    onClick={() => finalizeDefense(def.id)}
                    style={{
                      padding: "6px 12px",
                      background: "#16a34a",
                      color: "white",
                      borderRadius: "4px",
                      border: "none",
                      cursor: "pointer",
                    }}
                  >
                    Finalize
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
